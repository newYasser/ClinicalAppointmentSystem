using ClinicalAppointmentSystem.Application.Common.Pagination;

namespace ClinicalAppointmentSystem.Application.Appointments;

public interface IAppointmentService
{
    Task<PagedResult<AppointmentListItemDto>> GetListAsync(
        AppointmentListQuery query,
        CancellationToken cancellationToken = default);
}
