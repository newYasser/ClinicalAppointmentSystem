namespace ClinicalAppointmentSystem.Application.Appointments;

public sealed record ClinicSlotsDto(int DurationMinutes, IReadOnlyList<TimeOnly> Slots);
