namespace ClinicalAppointmentSystem.Application.Authentication;

public sealed record GoogleAuthSettings(string ClientId);

public sealed record GoogleUser(
    string Subject,
    string Email,
    bool EmailVerified,
    string DisplayName,
    string? PictureUrl);

public interface IGoogleTokenValidator
{
    Task<GoogleUser> ValidateAsync(string idToken, CancellationToken cancellationToken = default);
}
