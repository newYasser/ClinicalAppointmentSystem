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

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AppointmentDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppointmentDetailDto>> GetById(
        int id,
        CancellationToken cancellationToken) =>
        Ok(await appointments.GetByIdAsync(id, cancellationToken));
}
