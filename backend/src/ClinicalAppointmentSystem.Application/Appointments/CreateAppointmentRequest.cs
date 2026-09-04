using System.ComponentModel.DataAnnotations;

namespace ClinicalAppointmentSystem.Application.Appointments;

public sealed class CreateAppointmentRequest
{
    // Nullable so an omitted field is distinguishable from 0 or default.
    [Required(ErrorMessage = "Patient is required.")]
    public int? PatientId { get; set; }

    [Required(ErrorMessage = "Doctor is required.")]
    public int? DoctorId { get; set; }

    [Required(ErrorMessage = "Appointment date is required.")]
    public DateOnly? Date { get; set; }

    [Required(ErrorMessage = "Appointment time is required.")]
    public TimeOnly? StartTime { get; set; }

    [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
    public string? Notes { get; set; }
}
