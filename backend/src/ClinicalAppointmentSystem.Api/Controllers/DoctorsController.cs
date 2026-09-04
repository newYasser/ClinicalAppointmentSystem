using ClinicalAppointmentSystem.Application.Common.Pagination;
using ClinicalAppointmentSystem.Application.Doctors;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalAppointmentSystem.Api.Controllers;

public sealed class DoctorsController(IDoctorService doctors) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<DoctorListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<DoctorListItemDto>>> GetList(
        [FromQuery] DoctorListQuery query,
        CancellationToken cancellationToken) =>
        Ok(await doctors.GetListAsync(query, cancellationToken));
}
