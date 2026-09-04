using ClinicalAppointmentSystem.Application.Appointments;
using ClinicalAppointmentSystem.Application.Common.Pagination;
using ClinicalAppointmentSystem.Application.Patients;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalAppointmentSystem.Api.Controllers;

public sealed class PatientsController(IPatientService patients) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PatientListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<PatientListItemDto>>> GetList(
        [FromQuery] PatientListQuery query,
        CancellationToken cancellationToken) =>
        Ok(await patients.GetListAsync(query, cancellationToken));

    [HttpGet("lookup")]
    [ProducesResponseType(typeof(IReadOnlyList<PatientLookupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<PatientLookupDto>>> GetLookup(
        [FromQuery] PatientLookupQuery query,
        CancellationToken cancellationToken) =>
        Ok(await patients.GetLookupAsync(query, cancellationToken));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PatientDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDetailDto>> GetById(
        int id,
        CancellationToken cancellationToken) =>
        Ok(await patients.GetByIdAsync(id, cancellationToken));

    [HttpGet("{id:int}/appointments")]
    [ProducesResponseType(typeof(PagedResult<AppointmentListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<AppointmentListItemDto>>> GetAppointments(
        int id,
        [FromQuery] PatientAppointmentsQuery query,
        CancellationToken cancellationToken) =>
        Ok(await patients.GetAppointmentsAsync(id, query, cancellationToken));
}
