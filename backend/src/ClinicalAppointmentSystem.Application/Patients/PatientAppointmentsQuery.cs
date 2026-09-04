using ClinicalAppointmentSystem.Application.Common.Pagination;
using ClinicalAppointmentSystem.Domain.Enums;

namespace ClinicalAppointmentSystem.Application.Patients;

public sealed class PatientAppointmentsQuery : PageRequest
{
    public AppointmentStatus? Status { get; set; }
}
