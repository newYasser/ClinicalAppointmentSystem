using System.ComponentModel.DataAnnotations;

namespace ClinicalAppointmentSystem.Application.Doctors;

public sealed class DoctorAvailabilityQuery
{
    [Required(ErrorMessage = "Date is required.")]
    public DateOnly? Date { get; set; }

    public int? PatientId { get; set; }

    public int? ExcludeAppointmentId { get; set; }
}
