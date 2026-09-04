namespace ClinicalAppointmentSystem.Application.Patients;

public sealed record PatientListItemDto(
    int Id,
    string FirstName,
    string LastName,
    string FullName,
    DateOnly DateOfBirth,
    string Phone,
    string Email,
    int AppointmentCount);
