namespace ClinicalAppointmentSystem.Application.Doctors;

public sealed record DoctorLookupDto(int Id, string Label, int SpecialtyId, string Specialty);
