namespace ClinicalAppointmentSystem.Application.Dashboard;

public sealed record DashboardSummaryDto(
    int TotalPatients,
    int TotalDoctors,
    int TodayAppointmentCount,
    int UpcomingAppointmentCount,
    int SpecialtyCount,
    DateOnly Today,
    IReadOnlyList<DashboardAppointmentDto> TodaySchedule);
