using ClinicalAppointmentSystem.Domain.Common;

namespace ClinicalAppointmentSystem.Domain.Entities;

public class Doctor : AuditableEntity
{
    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public int SpecialtyId { get; private set; }

    public Specialty Specialty { get; private set; } = null!;

    public ICollection<Appointment> Appointments { get; } = [];

    public string FullName => $"Dr. {FirstName} {LastName}";
}
