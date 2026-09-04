using ClinicalAppointmentSystem.Domain.Enums;

namespace ClinicalAppointmentSystem.Application.Dashboard;

public sealed record DashboardAppointmentDto(
    int Id,
    TimeOnly StartTime,
    int PatientId,
    string PatientName,
    int DoctorId,
    string DoctorName,
    string Specialty,
    AppointmentStatus Status);
