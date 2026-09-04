using ClinicalAppointmentSystem.Application.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalAppointmentSystem.Api.Controllers;

public sealed class DashboardController(IDashboardService dashboard) : ApiControllerBase
{
    [HttpGet("summary")]
    [ProducesResponseType(typeof(DashboardSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary(
        CancellationToken cancellationToken) =>
        Ok(await dashboard.GetSummaryAsync(cancellationToken));
}
