namespace ClinicalAppointmentSystem.Domain.Common;

public abstract class AuditableEntity
{
    public int Id { get; private set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
