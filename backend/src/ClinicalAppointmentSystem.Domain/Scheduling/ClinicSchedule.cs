using ClinicalAppointmentSystem.Domain.Common;
using ClinicalAppointmentSystem.Domain.Exceptions;

namespace ClinicalAppointmentSystem.Domain.Scheduling;

public static class ClinicSchedule
{
    public const int SlotDurationMinutes = 30;

    public static readonly TimeOnly OpeningTime = new(8, 0);

    public static readonly TimeOnly LastSlotStart = new(17, 30);

    public static IReadOnlyList<TimeOnly> Slots { get; } = BuildSlots();

    public static bool IsValidSlotStart(TimeOnly startTime) =>
        startTime >= OpeningTime
        && startTime <= LastSlotStart
        && startTime.Second == 0
        && startTime.Millisecond == 0
        && startTime.Minute % SlotDurationMinutes == 0;

    public static void EnsureValidSlotStart(TimeOnly startTime)
    {
        if (!IsValidSlotStart(startTime))
        {
            throw new DomainValidationException(
                ErrorCodes.SlotOutOfBounds,
                $"Appointments start on the half hour between {OpeningTime:HH\\:mm} and {LastSlotStart:HH\\:mm}.");
        }
    }

    public static bool IsInPast(DateTime scheduledAt, DateTime nowLocal) => scheduledAt < nowLocal;

    private static TimeOnly[] BuildSlots()
    {
        var slots = new List<TimeOnly>();
        for (var t = OpeningTime; t <= LastSlotStart; t = t.AddMinutes(SlotDurationMinutes))
        {
            slots.Add(t);
        }

        return [.. slots];
    }
}
