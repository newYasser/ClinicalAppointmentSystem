using ClinicalAppointmentSystem.Application.Common.Pagination;

namespace ClinicalAppointmentSystem.Application.Doctors;

public interface IDoctorService
{
    Task<PagedResult<DoctorListItemDto>> GetListAsync(
        DoctorListQuery query,
        CancellationToken cancellationToken = default);
}
