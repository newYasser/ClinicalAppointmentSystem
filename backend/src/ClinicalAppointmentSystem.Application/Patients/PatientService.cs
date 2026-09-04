using ClinicalAppointmentSystem.Application.Appointments;
using ClinicalAppointmentSystem.Application.Common;
using ClinicalAppointmentSystem.Application.Common.Abstractions;
using ClinicalAppointmentSystem.Application.Common.Pagination;
using ClinicalAppointmentSystem.Domain.Common;
using ClinicalAppointmentSystem.Domain.Entities;
using ClinicalAppointmentSystem.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ClinicalAppointmentSystem.Application.Patients;

public sealed class PatientService(IClinicDbContext db, IClinicClock clock) : IPatientService
{
    private const string SortLastName = "lastName";
    private const string SortFirstName = "firstName";
    private const string SortDateOfBirth = "dateOfBirth";
    private const string SortAppointmentCount = "appointmentCount";
    private const string SortScheduledAt = "scheduledAt";

    public async Task<PagedResult<PatientListItemDto>> GetListAsync(
        PatientListQuery query,
        CancellationToken cancellationToken = default)
    {
        query.EnsureValid();

        var sortBy = query.ResolveSortBy(
            SortLastName,
            SortLastName,
            SortFirstName,
            SortDateOfBirth,
            SortAppointmentCount);

        var patients = db.Patients.AsNoTracking();

        var pattern = SearchTerm.ToLikePattern(query.Search);
        if (pattern is not null)
        {
            patients = patients.Where(p =>
                EF.Functions.Like(p.FirstName, pattern)
                || EF.Functions.Like(p.LastName, pattern)
                || EF.Functions.Like(p.FirstName + " " + p.LastName, pattern)
                || EF.Functions.Like(p.Phone, pattern)
                || EF.Functions.Like(p.Email, pattern));
        }


        var projected = Sort(patients, sortBy, query.IsDescending)
            .Select(p => new PatientListItemDto(
                p.Id,
                p.FirstName,
                p.LastName,
                p.FirstName + " " + p.LastName,
                p.DateOfBirth,
                p.Phone,
                p.Email,
                p.Appointments.Count));

        return await projected.ToPagedResultAsync(query.Page, query.PageSize, cancellationToken);
    }

    public async Task<PatientDetailDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var patient = await db.Patients
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new
            {
                p.Id,
                p.FirstName,
                p.LastName,
                p.DateOfBirth,
                p.Phone,
                p.Email,
                p.CreatedAt,
                p.UpdatedAt,
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(
                ErrorCodes.PatientNotFound,
                $"Patient {id} was not found.");

        var rows = await db.Appointments
            .AsNoTracking()
            .Where(a => a.PatientId == id)
            .OrderByDescending(a => a.ScheduledAt)
            .ToRows()
            .ToListAsync(cancellationToken);

        var nowLocal = clock.NowLocal;
        var appointments = rows.Select(row => row.ToDto(nowLocal)).ToList();

        return new PatientDetailDto(
            patient.Id,
            patient.FirstName,
            patient.LastName,
            $"{patient.FirstName} {patient.LastName}",
            patient.DateOfBirth,
            patient.Phone,
            patient.Email,
            appointments.Count,
            patient.CreatedAt,
            patient.UpdatedAt,
            appointments);
    }

    public async Task<PagedResult<AppointmentListItemDto>> GetAppointmentsAsync(
        int id,
        PatientAppointmentsQuery query,
        CancellationToken cancellationToken = default)
    {
        query.EnsureValid();
        _ = query.ResolveSortBy(SortScheduledAt, SortScheduledAt);

        var patientExists = await db.Patients
            .AsNoTracking()
            .AnyAsync(p => p.Id == id, cancellationToken);

        if (!patientExists)
        {
            throw new NotFoundException(
                ErrorCodes.PatientNotFound,
                $"Patient {id} was not found.");
        }

        var appointments = db.Appointments
            .AsNoTracking()
            .Where(a => a.PatientId == id);

        if (query.Status is { } status)
        {
            appointments = appointments.Where(a => a.Status == status);
        }

        // Newest first unless the caller explicitly asks for ascending.
        var ordered = query.SortDir is null || query.IsDescending
            ? appointments.OrderByDescending(a => a.ScheduledAt)
            : appointments.OrderBy(a => a.ScheduledAt);

        var page = await ordered
            .ToRows()
            .ToPagedResultAsync(query.Page, query.PageSize, cancellationToken);

        var nowLocal = clock.NowLocal;
        return page.Map(row => row.ToDto(nowLocal));
    }

    private static IQueryable<Patient> Sort(
        IQueryable<Patient> query,
        string sortBy,
        bool descending) =>
        (sortBy, descending) switch
        {
            (SortFirstName, false) => query.OrderBy(p => p.FirstName).ThenBy(p => p.LastName),
            (SortFirstName, true) => query.OrderByDescending(p => p.FirstName).ThenByDescending(p => p.LastName),
            (SortDateOfBirth, false) => query.OrderBy(p => p.DateOfBirth).ThenBy(p => p.Id),
            (SortDateOfBirth, true) => query.OrderByDescending(p => p.DateOfBirth).ThenBy(p => p.Id),
            (SortAppointmentCount, false) => query.OrderBy(p => p.Appointments.Count).ThenBy(p => p.Id),
            (SortAppointmentCount, true) => query.OrderByDescending(p => p.Appointments.Count).ThenBy(p => p.Id),
            (_, true) => query.OrderByDescending(p => p.LastName).ThenByDescending(p => p.FirstName),
            _ => query.OrderBy(p => p.LastName).ThenBy(p => p.FirstName),
        };
}
