using ClinicalAppointmentSystem.Domain.Entities;
using ClinicalAppointmentSystem.Domain.Enums;

namespace ClinicalAppointmentSystem.Application.Appointments;

public sealed record AppointmentRow(
    int Id,
    DateTime ScheduledAt,
    int DurationMinutes,
    AppointmentStatus Status,
    string? Notes,
    int PatientId,
    string PatientFirstName,
    string PatientLastName,
    int DoctorId,
    string DoctorFirstName,
    string DoctorLastName,
    string Specialty);

public static class AppointmentProjection
{

    public static IQueryable<AppointmentRow> ToRows(this IQueryable<Appointment> query) =>
        query.Select(a => new AppointmentRow(
            a.Id,
            a.ScheduledAt,
            a.DurationMinutes,
            a.Status,
            a.Notes,
            a.PatientId,
            a.Patient.FirstName,
            a.Patient.LastName,
            a.DoctorId,
            a.Doctor.FirstName,
            a.Doctor.LastName,
            a.Doctor.Specialty.Name));

    public static AppointmentListItemDto ToDto(this AppointmentRow row, DateTime nowLocal)
    {
        var startTime = TimeOnly.FromDateTime(row.ScheduledAt);
        var isScheduled = row.Status == AppointmentStatus.Scheduled;

        return new AppointmentListItemDto(
            row.Id,
            DateOnly.FromDateTime(row.ScheduledAt),
            startTime,
            startTime.AddMinutes(row.DurationMinutes),
            row.DurationMinutes,
            row.Status,
            row.Notes,
            row.PatientId,
            $"{row.PatientFirstName} {row.PatientLastName}",
            row.DoctorId,
            $"Dr. {row.DoctorFirstName} {row.DoctorLastName}",
            row.Specialty,
            isScheduled,
            isScheduled,
            row.ScheduledAt < nowLocal);
    }
}
