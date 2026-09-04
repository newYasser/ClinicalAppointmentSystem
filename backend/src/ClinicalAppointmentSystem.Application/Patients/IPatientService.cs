using ClinicalAppointmentSystem.Application.Common.Pagination;

namespace ClinicalAppointmentSystem.Application.Patients;

public interface IPatientService
{
    Task<PagedResult<PatientListItemDto>> GetListAsync(
        PatientListQuery query,
        CancellationToken cancellationToken = default);

    Task<PatientDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
