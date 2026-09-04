using ClinicalAppointmentSystem.Application.Common.Abstractions;

namespace ClinicalAppointmentSystem.Infrastructure.Time;

public sealed class ClinicClock : IClinicClock
{
    private readonly TimeProvider timeProvider;
    private readonly TimeZoneInfo timeZone;

    public ClinicClock(TimeProvider timeProvider, string timeZoneId)
    {
        this.timeProvider = timeProvider;
        timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    }

    public DateTime UtcNow => timeProvider.GetUtcNow().UtcDateTime;

    public DateTime NowLocal => TimeZoneInfo.ConvertTimeFromUtc(UtcNow, timeZone);

    public DateOnly Today => DateOnly.FromDateTime(NowLocal);
}
