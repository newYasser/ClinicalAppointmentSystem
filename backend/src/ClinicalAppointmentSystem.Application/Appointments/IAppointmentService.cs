using ClinicalAppointmentSystem.Application.Common.Pagination;
using ClinicalAppointmentSystem.Application.Doctors;

namespace ClinicalAppointmentSystem.Application.Appointments;

public interface IAppointmentService
{
    Task<PagedResult<AppointmentListItemDto>> GetListAsync(
        AppointmentListQuery query,
        CancellationToken cancellationToken = default);

    Task<AppointmentDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<AppointmentDetailDto> CreateAsync(
        CreateAppointmentRequest request,
        CancellationToken cancellationToken = default);

    Task<AppointmentDetailDto> UpdateAsync(
        int id,
        UpdateAppointmentRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<AppointmentDetailDto> CancelAsync(int id, CancellationToken cancellationToken = default);

    Task<AppointmentDetailDto> CompleteAsync(int id, CancellationToken cancellationToken = default);

    Task<DayBoardDto> GetDayBoardAsync(
        DayBoardQuery query,
        CancellationToken cancellationToken = default);

    Task<DoctorAvailabilityDto> GetDoctorAvailabilityAsync(
        int doctorId,
        DoctorAvailabilityQuery query,
        CancellationToken cancellationToken = default);
}
