using ClinicalAppointmentSystem.Application.Common.Pagination;
using ClinicalAppointmentSystem.Domain.Enums;

namespace ClinicalAppointmentSystem.Application.Appointments;

public sealed class AppointmentListQuery : PageRequest
{
    public string? Search { get; set; }

    public DateOnly? Date { get; set; }

    public DateOnly? From { get; set; }

    public DateOnly? To { get; set; }

    public int? DoctorId { get; set; }

    public int? PatientId { get; set; }

    public AppointmentStatus? Status { get; set; }
}
