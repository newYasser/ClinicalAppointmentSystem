namespace ClinicalAppointmentSystem.Application.Patients;

public sealed class PatientLookupQuery
{
    public const int DefaultLimit = 500;
    public const int MaxLimit = 1000;

    public string? Search { get; set; }

    public int Limit { get; set; } = DefaultLimit;
}
