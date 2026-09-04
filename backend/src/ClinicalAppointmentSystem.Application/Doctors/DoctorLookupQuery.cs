namespace ClinicalAppointmentSystem.Application.Doctors;

public sealed class DoctorLookupQuery
{
    public string? Search { get; set; }

    public int? SpecialtyId { get; set; }
}
