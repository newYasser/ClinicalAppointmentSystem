using ClinicalAppointmentSystem.Domain.Enums;

namespace ClinicalAppointmentSystem.Application.Appointments;

// One cohesive response shape, kept in a single file.
public sealed record DayBoardDto(
    DateOnly Date,
    IReadOnlyList<DayBoardDoctorDto> Doctors,
    IReadOnlyList<DayBoardRowDto> Rows);

public sealed record DayBoardDoctorDto(int Id, string Name, string Specialty);

public sealed record DayBoardRowDto(TimeOnly StartTime, IReadOnlyList<DayBoardCellDto> Cells);

public sealed record DayBoardCellDto(
    int DoctorId,
    DayBoardCellState State,
    int? AppointmentId,
    int? PatientId,
    string? Label,
    AppointmentStatus? Status);

public enum DayBoardCellState
{
    Free,
    Past,
    Booked,
    Completed,
}
