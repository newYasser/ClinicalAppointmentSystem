namespace ClinicalAppointmentSystem.Application.Common.Abstractions;

public interface IClinicClock
{
    DateTime NowLocal { get; }

    DateOnly Today { get; }

    DateTime UtcNow { get; }
}
