using ClinicalAppointmentSystem.Application.Appointments;
using ClinicalAppointmentSystem.Application.Common.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalAppointmentSystem.Api.Controllers;

public sealed class AppointmentsController(IAppointmentService appointments) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AppointmentListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<AppointmentListItemDto>>> GetList(
        [FromQuery] AppointmentListQuery query,
        CancellationToken cancellationToken) =>
        Ok(await appointments.GetListAsync(query, cancellationToken));
}
