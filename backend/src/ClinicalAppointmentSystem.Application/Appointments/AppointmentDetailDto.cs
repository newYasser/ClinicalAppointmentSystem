using ClinicalAppointmentSystem.Domain.Enums;

namespace ClinicalAppointmentSystem.Application.Appointments;

public sealed record AppointmentDetailDto(
    int Id,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int DurationMinutes,
    AppointmentStatus Status,
    string? Notes,
    int PatientId,
    string PatientName,
    int DoctorId,
    string DoctorName,
    string Specialty,
    bool CanComplete,
    bool CanCancel,
    bool IsPast,
    DateTime CreatedAt,
    DateTime UpdatedAt);
