namespace ClinicalAppointmentSystem.Domain.Entities;

public class Specialty
{
    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public ICollection<Doctor> Doctors { get; } = [];
}
