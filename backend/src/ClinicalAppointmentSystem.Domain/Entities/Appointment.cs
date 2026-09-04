using ClinicalAppointmentSystem.Domain.Common;
using ClinicalAppointmentSystem.Domain.Enums;
using ClinicalAppointmentSystem.Domain.Exceptions;
using ClinicalAppointmentSystem.Domain.Scheduling;

namespace ClinicalAppointmentSystem.Domain.Entities;

public class Appointment : AuditableEntity
{
    private Appointment()
    {
    }

    public static Appointment Schedule(
        int patientId,
        int doctorId,
        DateOnly date,
        TimeOnly startTime,
        string? notes)
    {
        ClinicSchedule.EnsureValidSlotStart(startTime);

        var appointment = new Appointment
        {
            PatientId = patientId,
            DoctorId = doctorId,
            ScheduledAt = date.ToDateTime(startTime),
            DurationMinutes = ClinicSchedule.SlotDurationMinutes,
            Status = AppointmentStatus.Scheduled,
            Notes = Normalise(notes),
        };

        appointment.SyncActiveSlot();
        return appointment;
    }

    public int PatientId { get; private set; }

    public Patient Patient { get; private set; } = null!;

    public int DoctorId { get; private set; }

    public Doctor Doctor { get; private set; } = null!;

    public DateTime ScheduledAt { get; private set; }

    public int DurationMinutes { get; private set; } = ClinicSchedule.SlotDurationMinutes;

    public AppointmentStatus Status { get; private set; }

    public string? Notes { get; private set; }

    public DateTime? ActiveSlot { get; private set; }

    public DateOnly Date => DateOnly.FromDateTime(ScheduledAt);

    public TimeOnly StartTime => TimeOnly.FromDateTime(ScheduledAt);

    public TimeOnly EndTime => StartTime.AddMinutes(DurationMinutes);

    public bool IsLive => Status != AppointmentStatus.Cancelled;

    public bool CanCancel => Status == AppointmentStatus.Scheduled;

    public bool CanComplete => Status == AppointmentStatus.Scheduled;

    public void UpdateDetails(
        int patientId,
        int doctorId,
        DateOnly date,
        TimeOnly startTime,
        string? notes)
    {
        ClinicSchedule.EnsureValidSlotStart(startTime);

        PatientId = patientId;
        DoctorId = doctorId;
        ScheduledAt = date.ToDateTime(startTime);
        Notes = Normalise(notes);

        SyncActiveSlot();
    }

    public void Cancel()
    {
        if (Status != AppointmentStatus.Scheduled)
        {
            throw new ConflictException(
                ErrorCodes.InvalidStatusTransition,
                Status == AppointmentStatus.Cancelled
                    ? "This appointment is already cancelled."
                    : "A completed appointment cannot be cancelled.");
        }

        Status = AppointmentStatus.Cancelled;
        SyncActiveSlot();
    }

    public void Complete()
    {
        if (Status != AppointmentStatus.Scheduled)
        {
            throw new ConflictException(
                ErrorCodes.InvalidStatusTransition,
                Status == AppointmentStatus.Cancelled
                    ? "A cancelled appointment cannot be marked as completed."
                    : "This appointment is already completed.");
        }

        Status = AppointmentStatus.Completed;
        SyncActiveSlot();
    }

    private void SyncActiveSlot() => ActiveSlot = IsLive ? ScheduledAt : null;

    private static string? Normalise(string? notes) =>
        string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
}
