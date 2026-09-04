namespace ClinicalAppointmentSystem.Application.Doctors;

public sealed record DoctorListItemDto(
    int Id,
    string FirstName,
    string LastName,
    string FullName,
    int SpecialtyId,
    string Specialty,
    int AppointmentCount);
