using ClinicalAppointmentSystem.Application.Common.Pagination;

namespace ClinicalAppointmentSystem.Application.Doctors;

public interface IDoctorService
{
    Task<PagedResult<DoctorListItemDto>> GetListAsync(
        DoctorListQuery query,
        CancellationToken cancellationToken = default);

    Task<DoctorDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DoctorLookupDto>> GetLookupAsync(
        DoctorLookupQuery query,
        CancellationToken cancellationToken = default);
}
