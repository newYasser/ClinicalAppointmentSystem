namespace ClinicalAppointmentSystem.Application.Specialties;

public interface ISpecialtyService
{
    Task<IReadOnlyList<SpecialtyDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
