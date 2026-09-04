namespace ClinicalAppointmentSystem.Application.Doctors;

public sealed record DoctorAvailabilityDto(
    int DoctorId,
    string DoctorName,
    DateOnly Date,
    IReadOnlyList<AvailabilitySlotDto> Slots);

public sealed record AvailabilitySlotDto(
    TimeOnly StartTime,
    AvailabilitySlotState State,
    int? AppointmentId);

public enum AvailabilitySlotState
{
    Free,
    Past,
    TakenByDoctor,
    TakenByPatient,
}
