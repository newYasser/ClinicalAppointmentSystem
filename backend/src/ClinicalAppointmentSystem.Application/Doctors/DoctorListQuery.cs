using ClinicalAppointmentSystem.Application.Common.Pagination;

namespace ClinicalAppointmentSystem.Application.Doctors;

public sealed class DoctorListQuery : PageRequest
{
    public string? Search { get; set; }

    public int? SpecialtyId { get; set; }

    public string? Specialty { get; set; }
}
