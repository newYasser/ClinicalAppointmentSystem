using ClinicalAppointmentSystem.Application.Appointments;
using ClinicalAppointmentSystem.Application.Common.Pagination;
using ClinicalAppointmentSystem.Application.Doctors;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalAppointmentSystem.Api.Controllers;

public sealed class DoctorsController(
    IDoctorService doctors,
    IScheduleQueryService schedule) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<DoctorListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<DoctorListItemDto>>> GetList(
        [FromQuery] DoctorListQuery query,
        CancellationToken cancellationToken) =>
        Ok(await doctors.GetListAsync(query, cancellationToken));

    [HttpGet("lookup")]
    [ProducesResponseType(typeof(IReadOnlyList<DoctorLookupDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DoctorLookupDto>>> GetLookup(
        [FromQuery] DoctorLookupQuery query,
        CancellationToken cancellationToken) =>
        Ok(await doctors.GetLookupAsync(query, cancellationToken));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DoctorDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DoctorDetailDto>> GetById(
        int id,
        CancellationToken cancellationToken) =>
        Ok(await doctors.GetByIdAsync(id, cancellationToken));

    [HttpGet("{id:int}/availability")]
    [ProducesResponseType(typeof(DoctorAvailabilityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DoctorAvailabilityDto>> GetAvailability(
        int id,
        [FromQuery] DoctorAvailabilityQuery query,
        CancellationToken cancellationToken) =>
        Ok(await schedule.GetDoctorAvailabilityAsync(id, query, cancellationToken));
}
