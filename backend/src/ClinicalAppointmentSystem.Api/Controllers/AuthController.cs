using ClinicalAppointmentSystem.Application.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalAppointmentSystem.Api.Controllers;

[AllowAnonymous]
public sealed class AuthController(IAuthService auth) : ApiControllerBase
{
    [HttpPost("google")]
    [ProducesResponseType(typeof(SignInResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SignInResultDto>> SignInWithGoogle(
        [FromBody] GoogleSignInRequest request,
        CancellationToken cancellationToken) =>
        Ok(await auth.SignInWithGoogleAsync(request, cancellationToken));
}
