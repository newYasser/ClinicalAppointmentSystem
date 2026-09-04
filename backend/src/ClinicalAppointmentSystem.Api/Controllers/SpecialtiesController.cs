using ClinicalAppointmentSystem.Application.Specialties;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalAppointmentSystem.Api.Controllers;

public sealed class SpecialtiesController(ISpecialtyService specialties) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SpecialtyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SpecialtyDto>>> GetAll(
        CancellationToken cancellationToken) =>
        Ok(await specialties.GetAllAsync(cancellationToken));
}
