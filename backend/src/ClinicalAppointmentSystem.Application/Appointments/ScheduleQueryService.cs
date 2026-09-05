using System.Linq.Expressions;
using ClinicalAppointmentSystem.Application.Common.Abstractions;
using ClinicalAppointmentSystem.Application.Doctors;
using ClinicalAppointmentSystem.Domain.Common;
using ClinicalAppointmentSystem.Domain.Entities;
using ClinicalAppointmentSystem.Domain.Enums;
using ClinicalAppointmentSystem.Domain.Exceptions;
using ClinicalAppointmentSystem.Domain.Scheduling;
using Microsoft.EntityFrameworkCore;

namespace ClinicalAppointmentSystem.Application.Appointments;

public sealed class ScheduleQueryService(IClinicDbContext db, IClinicClock clock) : IScheduleQueryService
{
    public ClinicSlotsDto GetSlots() =>
        new(ClinicSchedule.SlotDurationMinutes, ClinicSchedule.Slots);

    public async Task<DayBoardDto> GetDayBoardAsync(
        DayBoardQuery query,
        CancellationToken cancellationToken = default)
    {
        var date = query.Date!.Value;

        var doctorQuery = db.Doctors.AsNoTracking();

        if (query.SpecialtyId is { } specialtyId)
        {
            doctorQuery = doctorQuery.Where(d => d.SpecialtyId == specialtyId);
        }

        if (query.DoctorId is { } filterDoctorId)
        {
            doctorQuery = doctorQuery.Where(d => d.Id == filterDoctorId);
        }

        var doctors = await doctorQuery
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .ThenBy(d => d.Id)
            .Select(d => new DayBoardDoctorDto(
                d.Id,
                "Dr. " + d.FirstName + " " + d.LastName,
                d.Specialty.Name))
            .ToListAsync(cancellationToken);

        var doctorIds = doctors.Select(d => d.Id).ToList();

        var live = await GetLiveSlotsAsync(
            date,
            a => doctorIds.Contains(a.DoctorId),
            cancellationToken);

        // At most one live appointment per (doctor, slot) — the unique index guarantees it.
        var occupied = live.ToDictionary(slot => (slot.DoctorId, slot.StartTime));

        var nowLocal = clock.NowLocal;

        var rows = ClinicSchedule.Slots
            .Select(slot =>
            {
                var slotIsPast = ClinicSchedule.IsInPast(date.ToDateTime(slot), nowLocal);

                var cells = doctors
                    .Select(doctor =>
                    {
                        if (occupied.TryGetValue((doctor.Id, slot), out var appointment))
                        {
                            return new DayBoardCellDto(
                                doctor.Id,
                                appointment.Status == AppointmentStatus.Completed
                                    ? DayBoardCellState.Completed
                                    : DayBoardCellState.Booked,
                                appointment.Id,
                                appointment.PatientId,
                                appointment.PatientName,
                                appointment.Status);
                        }

                        return new DayBoardCellDto(
                            doctor.Id,
                            slotIsPast ? DayBoardCellState.Past : DayBoardCellState.Free,
                            null,
                            null,
                            null,
                            null);
                    })
                    .ToList();

                return new DayBoardRowDto(slot, cells);
            })
            .ToList();

        return new DayBoardDto(date, doctors, rows);
    }

    public async Task<DoctorAvailabilityDto> GetDoctorAvailabilityAsync(
        int doctorId,
        DoctorAvailabilityQuery query,
        CancellationToken cancellationToken = default)
    {
        var date = query.Date!.Value;

        var doctorName = await db.Doctors
            .AsNoTracking()
            .Where(d => d.Id == doctorId)
            .Select(d => "Dr. " + d.FirstName + " " + d.LastName)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(
                ErrorCodes.DoctorNotFound,
                $"Doctor {doctorId} was not found.");

        if (query.PatientId is { } requestedPatientId
            && !await db.Patients.AnyAsync(p => p.Id == requestedPatientId, cancellationToken))
        {
            throw new NotFoundException(
                ErrorCodes.PatientNotFound,
                $"Patient {requestedPatientId} was not found.");
        }

        var patientId = query.PatientId;
        var excludeAppointmentId = query.ExcludeAppointmentId;

        var live = await GetLiveSlotsAsync(
            date,
            a => (a.DoctorId == doctorId || (patientId != null && a.PatientId == patientId))

                // When editing, the appointment must not count against its own slot.
                && (excludeAppointmentId == null || a.Id != excludeAppointmentId),
            cancellationToken);

        var takenByDoctor = live
            .Where(slot => slot.DoctorId == doctorId)
            .ToDictionary(slot => slot.StartTime);

        var takenByPatient = live
            .Where(slot => patientId != null && slot.PatientId == patientId)
            .ToDictionary(slot => slot.StartTime);

        var nowLocal = clock.NowLocal;

        var slots = ClinicSchedule.Slots
            .Select(slot =>
            {
                // Occupancy wins over Past: a booked slot in the past still shows as booked,
                // matching the day board.
                if (takenByDoctor.TryGetValue(slot, out var doctorAppointment))
                {
                    return new AvailabilitySlotDto(
                        slot,
                        AvailabilitySlotState.TakenByDoctor,
                        doctorAppointment.Id);
                }

                if (takenByPatient.TryGetValue(slot, out var patientAppointment))
                {
                    return new AvailabilitySlotDto(
                        slot,
                        AvailabilitySlotState.TakenByPatient,
                        patientAppointment.Id);
                }

                return new AvailabilitySlotDto(
                    slot,
                    ClinicSchedule.IsInPast(date.ToDateTime(slot), nowLocal)
                        ? AvailabilitySlotState.Past
                        : AvailabilitySlotState.Free,
                    null);
            })
            .ToList();

        return new DoctorAvailabilityDto(doctorId, doctorName, date, slots);
    }

    // The one query both views are built on: the day's live appointments, keyed by slot time.
    // LiveBetween matches on ActiveSlot, so cancelled appointments are absent and their slots
    // show as free — which is the point of cancelling.
    private async Task<List<LiveSlot>> GetLiveSlotsAsync(
        DateOnly date,
        Expression<Func<Appointment, bool>> filter,
        CancellationToken cancellationToken)
    {
        var dayStart = date.ToDateTime(TimeOnly.MinValue);

        var rows = await db.Appointments
            .AsNoTracking()
            .Where(AppointmentSpecifications.LiveBetween(dayStart, dayStart.AddDays(1)))
            .Where(filter)
            .Select(a => new
            {
                a.Id,
                a.DoctorId,
                a.PatientId,
                a.ActiveSlot,
                a.Status,
                PatientFirstName = a.Patient.FirstName,
                PatientLastName = a.Patient.LastName,
            })
            .ToListAsync(cancellationToken);

        return
        [
            .. rows.Select(row => new LiveSlot(
                row.Id,
                row.DoctorId,
                row.PatientId,
                TimeOnly.FromDateTime(row.ActiveSlot!.Value),
                row.Status,
                $"{row.PatientFirstName} {row.PatientLastName}")),
        ];
    }

    private sealed record LiveSlot(
        int Id,
        int DoctorId,
        int PatientId,
        TimeOnly StartTime,
        AppointmentStatus Status,
        string PatientName);
}
