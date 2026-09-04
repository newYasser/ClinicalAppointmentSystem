using System.Globalization;
using ClinicalAppointmentSystem.Application.Common;
using ClinicalAppointmentSystem.Application.Common.Abstractions;
using ClinicalAppointmentSystem.Application.Common.Pagination;
using ClinicalAppointmentSystem.Domain.Common;
using ClinicalAppointmentSystem.Domain.Entities;
using ClinicalAppointmentSystem.Domain.Exceptions;
using ClinicalAppointmentSystem.Domain.Scheduling;
using Microsoft.EntityFrameworkCore;

namespace ClinicalAppointmentSystem.Application.Appointments;

public sealed class AppointmentService(IClinicDbContext db, IClinicClock clock) : IAppointmentService
{
    private const string SortScheduledAt = "scheduledAt";
    private const string SortPatientName = "patientName";
    private const string SortDoctorName = "doctorName";
    private const string SortStatus = "status";

    public async Task<PagedResult<AppointmentListItemDto>> GetListAsync(
        AppointmentListQuery query,
        CancellationToken cancellationToken = default)
    {
        query.EnsureValid();

        var sortBy = query.ResolveSortBy(
            SortScheduledAt,
            SortScheduledAt,
            SortPatientName,
            SortDoctorName,
            SortStatus);

        var appointments = db.Appointments.AsNoTracking();

        if (query.Date is { } date)
        {
            var dayStart = date.ToDateTime(TimeOnly.MinValue);
            var nextDayStart = dayStart.AddDays(1);
            appointments = appointments.Where(a => a.ScheduledAt >= dayStart && a.ScheduledAt < nextDayStart);
        }
        else
        {
            if (query.From is { } from)
            {
                var fromStart = from.ToDateTime(TimeOnly.MinValue);
                appointments = appointments.Where(a => a.ScheduledAt >= fromStart);
            }

            if (query.To is { } to)
            {
                var afterTo = to.AddDays(1).ToDateTime(TimeOnly.MinValue);
                appointments = appointments.Where(a => a.ScheduledAt < afterTo);
            }
        }

        if (query.DoctorId is { } doctorId)
        {
            appointments = appointments.Where(a => a.DoctorId == doctorId);
        }

        if (query.PatientId is { } patientId)
        {
            appointments = appointments.Where(a => a.PatientId == patientId);
        }

        if (query.Status is { } status)
        {
            appointments = appointments.Where(a => a.Status == status);
        }

        var pattern = SearchTerm.ToLikePattern(query.Search);
        if (pattern is not null)
        {
            appointments = appointments.Where(a =>
                EF.Functions.Like(a.Patient.FirstName, pattern)
                || EF.Functions.Like(a.Patient.LastName, pattern)
                || EF.Functions.Like(a.Patient.FirstName + " " + a.Patient.LastName, pattern)
                || EF.Functions.Like(a.Doctor.FirstName, pattern)
                || EF.Functions.Like(a.Doctor.LastName, pattern)
                || EF.Functions.Like(a.Doctor.FirstName + " " + a.Doctor.LastName, pattern));
        }

        var page = await Sort(appointments, sortBy, query.IsDescending)
            .ToRows()
            .ToPagedResultAsync(query.Page, query.PageSize, cancellationToken);

        var nowLocal = clock.NowLocal;
        return page.Map(row => row.ToDto(nowLocal));
    }

    public async Task<AppointmentDetailDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var row = await db.Appointments
            .AsNoTracking()
            .Where(a => a.Id == id)
            .ToRows()
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(
                ErrorCodes.AppointmentNotFound,
                $"Appointment {id} was not found.");

        return row.ToDetailDto(clock.NowLocal);
    }

    public async Task<AppointmentDetailDto> CreateAsync(
        CreateAppointmentRequest request,
        CancellationToken cancellationToken = default)
    {
        // Presence is already enforced by DataAnnotations before the service is reached.
        var patientId = request.PatientId!.Value;
        var doctorId = request.DoctorId!.Value;
        var date = request.Date!.Value;
        var startTime = request.StartTime!.Value;

        // Checks run in the order the contract specifies and stop at the first failure:
        // slot bounds (400), unknown ids (404), past (409), then conflicts (409).
        ClinicSchedule.EnsureValidSlotStart(startTime);

        await EnsurePatientExistsAsync(patientId, cancellationToken);
        await EnsureDoctorExistsAsync(doctorId, cancellationToken);

        var scheduledAt = date.ToDateTime(startTime);

        EnsureNotInPast(scheduledAt);
        await EnsureSlotIsFreeAsync(scheduledAt, doctorId, patientId, null, cancellationToken);

        var appointment = Appointment.Schedule(patientId, doctorId, date, startTime, request.Notes);

        db.Appointments.Add(appointment);
        await db.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(appointment.Id, cancellationToken);
    }

    public async Task<AppointmentDetailDto> UpdateAsync(
        int id,
        UpdateAppointmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var patientId = request.PatientId!.Value;
        var doctorId = request.DoctorId!.Value;
        var date = request.Date!.Value;
        var startTime = request.StartTime!.Value;

        var appointment = await db.Appointments
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken)
            ?? throw new NotFoundException(
                ErrorCodes.AppointmentNotFound,
                $"Appointment {id} was not found.");

        ClinicSchedule.EnsureValidSlotStart(startTime);

        await EnsurePatientExistsAsync(patientId, cancellationToken);
        await EnsureDoctorExistsAsync(doctorId, cancellationToken);

        var scheduledAt = date.ToDateTime(startTime);

        // The past rule governs rescheduling. Applying it to an unchanged slot would make a
        // notes-only edit impossible on any appointment whose time has already passed.
        if (scheduledAt != appointment.ScheduledAt)
        {
            EnsureNotInPast(scheduledAt);
        }

        // A cancelled appointment holds no slot, so it cannot clash with anything. Completed
        // ones still hold theirs, so they are checked.
        if (appointment.IsLive)
        {
            await EnsureSlotIsFreeAsync(scheduledAt, doctorId, patientId, id, cancellationToken);
        }

        appointment.UpdateDetails(patientId, doctorId, date, startTime, request.Notes);

        await db.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    private async Task EnsurePatientExistsAsync(int patientId, CancellationToken cancellationToken)
    {
        if (!await db.Patients.AnyAsync(p => p.Id == patientId, cancellationToken))
        {
            throw new NotFoundException(
                ErrorCodes.PatientNotFound,
                $"Patient {patientId} was not found.");
        }
    }

    private async Task EnsureDoctorExistsAsync(int doctorId, CancellationToken cancellationToken)
    {
        if (!await db.Doctors.AnyAsync(d => d.Id == doctorId, cancellationToken))
        {
            throw new NotFoundException(
                ErrorCodes.DoctorNotFound,
                $"Doctor {doctorId} was not found.");
        }
    }

    private void EnsureNotInPast(DateTime scheduledAt)
    {
        if (ClinicSchedule.IsInPast(scheduledAt, clock.NowLocal))
        {
            throw new ConflictException(
                ErrorCodes.AppointmentInPast,
                "That slot is in the past. Appointments can only be scheduled from the current time onwards.");
        }
    }

    private async Task EnsureSlotIsFreeAsync(
        DateTime scheduledAt,
        int doctorId,
        int patientId,
        int? excludeAppointmentId,
        CancellationToken cancellationToken)
    {
        var clash = await db.Appointments
            .AsNoTracking()
            .Where(AppointmentSpecifications.ClashingWith(
                scheduledAt,
                doctorId,
                patientId,
                excludeAppointmentId))

            .OrderByDescending(a => a.DoctorId == doctorId)
            .Select(a => new { a.Id, a.DoctorId })
            .FirstOrDefaultAsync(cancellationToken);

        if (clash is null)
        {
            return;
        }

        var when = $"{TimeOnly.FromDateTime(scheduledAt):HH\\:mm} on "
            + DateOnly.FromDateTime(scheduledAt).ToString("ddd d MMM yyyy", CultureInfo.InvariantCulture);

        if (clash.DoctorId == doctorId)
        {
            var doctorName = await db.Doctors
                .Where(d => d.Id == doctorId)
                .Select(d => "Dr. " + d.FirstName + " " + d.LastName)
                .FirstAsync(cancellationToken);

            throw new ConflictException(
                ErrorCodes.DoctorSlotConflict,
                $"{doctorName} already has an appointment at {when}. Each appointment is 30 minutes and may not overlap — pick a free slot.")
                .With("conflictingAppointmentId", clash.Id);
        }

        var patientName = await db.Patients
            .Where(p => p.Id == patientId)
            .Select(p => p.FirstName + " " + p.LastName)
            .FirstAsync(cancellationToken);

        throw new ConflictException(
            ErrorCodes.PatientSlotConflict,
            $"{patientName} already has an appointment at {when}. Each appointment is 30 minutes and may not overlap — pick a free slot.")
            .With("conflictingAppointmentId", clash.Id);
    }

    private static IQueryable<Appointment> Sort(
        IQueryable<Appointment> query,
        string sortBy,
        bool descending) =>
        (sortBy, descending) switch
        {
            (SortPatientName, false) => query.OrderBy(a => a.Patient.LastName).ThenBy(a => a.Patient.FirstName).ThenBy(a => a.Id),
            (SortPatientName, true) => query.OrderByDescending(a => a.Patient.LastName).ThenByDescending(a => a.Patient.FirstName).ThenBy(a => a.Id),
            (SortDoctorName, false) => query.OrderBy(a => a.Doctor.LastName).ThenBy(a => a.Doctor.FirstName).ThenBy(a => a.Id),
            (SortDoctorName, true) => query.OrderByDescending(a => a.Doctor.LastName).ThenByDescending(a => a.Doctor.FirstName).ThenBy(a => a.Id),
            (SortStatus, false) => query.OrderBy(a => a.Status).ThenBy(a => a.ScheduledAt).ThenBy(a => a.Id),
            (SortStatus, true) => query.OrderByDescending(a => a.Status).ThenBy(a => a.ScheduledAt).ThenBy(a => a.Id),
            (_, true) => query.OrderByDescending(a => a.ScheduledAt).ThenBy(a => a.Id),
            _ => query.OrderBy(a => a.ScheduledAt).ThenBy(a => a.Id),
        };
}
