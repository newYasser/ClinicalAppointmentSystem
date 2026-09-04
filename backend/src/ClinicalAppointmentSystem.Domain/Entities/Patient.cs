using ClinicalAppointmentSystem.Domain.Common;

namespace ClinicalAppointmentSystem.Domain.Entities;

public class Patient : AuditableEntity
{
    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public DateOnly DateOfBirth { get; private set; }

    public string Phone { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public ICollection<Appointment> Appointments { get; } = [];

    public string FullName => $"{FirstName} {LastName}";
}
