using ClinicalAppointmentSystem.Application.Appointments;

namespace ClinicalAppointmentSystem.Application.Patients;

public sealed record PatientDetailDto(
    int Id,
    string FirstName,
    string LastName,
    string FullName,
    DateOnly DateOfBirth,
    string Phone,
    string Email,
    int AppointmentCount,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<AppointmentListItemDto> Appointments);
