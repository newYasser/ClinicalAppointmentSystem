using ClinicalAppointmentSystem.Application.Common.Pagination;

namespace ClinicalAppointmentSystem.Application.Patients;

public sealed class PatientListQuery : PageRequest
{
    public string? Search { get; set; }
}
