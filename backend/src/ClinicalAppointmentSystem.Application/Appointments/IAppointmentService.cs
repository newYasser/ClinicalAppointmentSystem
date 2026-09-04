using ClinicalAppointmentSystem.Application.Common.Pagination;

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
}
