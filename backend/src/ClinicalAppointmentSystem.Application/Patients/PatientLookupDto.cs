namespace ClinicalAppointmentSystem.Application.Patients;

public sealed record PatientLookupDto(int Id, string Label, DateOnly DateOfBirth);
