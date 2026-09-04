namespace ClinicalAppointmentSystem.Application.Doctors;

public sealed record DoctorDetailDto(
    int Id,
    string FirstName,
    string LastName,
    string FullName,
    int SpecialtyId,
    string Specialty,
    int AppointmentCount,
    DateTime CreatedAt,
    DateTime UpdatedAt);
