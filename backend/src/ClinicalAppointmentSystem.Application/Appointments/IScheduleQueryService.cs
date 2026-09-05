using ClinicalAppointmentSystem.Application.Doctors;

namespace ClinicalAppointmentSystem.Application.Appointments;

public interface IScheduleQueryService
{
    ClinicSlotsDto GetSlots();

    Task<DayBoardDto> GetDayBoardAsync(
        DayBoardQuery query,
        CancellationToken cancellationToken = default);

    Task<DoctorAvailabilityDto> GetDoctorAvailabilityAsync(
        int doctorId,
        DoctorAvailabilityQuery query,
        CancellationToken cancellationToken = default);
}
