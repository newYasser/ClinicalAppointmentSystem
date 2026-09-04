using System.ComponentModel.DataAnnotations;

namespace ClinicalAppointmentSystem.Application.Appointments;

public sealed class DayBoardQuery
{
    [Required(ErrorMessage = "Date is required.")]
    public DateOnly? Date { get; set; }

    public int? SpecialtyId { get; set; }

    public int? DoctorId { get; set; }
}
