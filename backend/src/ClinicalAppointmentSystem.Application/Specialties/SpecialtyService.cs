using ClinicalAppointmentSystem.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ClinicalAppointmentSystem.Application.Specialties;

public sealed class SpecialtyService(IClinicDbContext db) : ISpecialtyService
{
    public async Task<IReadOnlyList<SpecialtyDto>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        await db.Specialties
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new SpecialtyDto(s.Id, s.Name, s.Doctors.Count))
            .ToListAsync(cancellationToken);
}
