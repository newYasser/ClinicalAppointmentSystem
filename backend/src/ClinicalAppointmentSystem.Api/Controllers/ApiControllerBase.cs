using Microsoft.AspNetCore.Mvc;

namespace ClinicalAppointmentSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase;
