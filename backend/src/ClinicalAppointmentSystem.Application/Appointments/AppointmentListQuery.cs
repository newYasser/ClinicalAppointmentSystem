using ClinicalAppointmentSystem.Application.Common.Pagination;
using ClinicalAppointmentSystem.Domain.Enums;
using ClinicalAppointmentSystem.Domain.Exceptions;

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

    public override void EnsureValid()
    {
        base.EnsureValid();

        if (Date is not null && (From is not null || To is not null))
        {
            throw DomainValidationException.ForField(
                "Date",
                "Filter by either date or from/to, not both.");
        }

        if (From is { } from && To is { } to && to < from)
        {
            throw DomainValidationException.ForField("To", "to must be on or after from.");
        }
    }
}
