namespace ClinicalAppointmentSystem.Application.Authentication;

public interface IAuthService
{
    AuthConfigDto GetConfig();

    Task<SignInResultDto> SignInWithGoogleAsync(
        GoogleSignInRequest request,
        CancellationToken cancellationToken = default);
}
