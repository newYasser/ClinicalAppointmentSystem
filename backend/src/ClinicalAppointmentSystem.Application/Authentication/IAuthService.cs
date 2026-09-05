namespace ClinicalAppointmentSystem.Application.Authentication;

public interface IAuthService
{
    Task<SignInResultDto> SignInWithGoogleAsync(
        GoogleSignInRequest request,
        CancellationToken cancellationToken = default);
}
