namespace ClinicalAppointmentSystem.Application.Authentication;

public sealed class AuthService(
    IGoogleTokenValidator googleTokens,
    IAccessTokenIssuer accessTokens,
    GoogleAuthSettings googleSettings) : IAuthService
{
    private const string BearerTokenType = "Bearer";

    // The client ID is public by design — Google Identity Services needs it in the
    // browser. Serving it keeps one source of truth instead of a second copy in the
    // Angular build.
    public AuthConfigDto GetConfig() => new(googleSettings.ClientId);

    public async Task<SignInResultDto> SignInWithGoogleAsync(
        GoogleSignInRequest request,
        CancellationToken cancellationToken = default)
    {
        var googleUser = await googleTokens.ValidateAsync(request.IdToken!, cancellationToken);

        var accessToken = accessTokens.Issue(new TokenSubject(
            googleUser.Subject,
            googleUser.Email,
            googleUser.DisplayName,
            googleUser.PictureUrl));

        return new SignInResultDto(
            accessToken.Value,
            BearerTokenType,
            accessToken.ExpiresInSeconds,
            accessToken.ExpiresAtUtc,
            new SignedInUserDto(
                googleUser.Subject,
                googleUser.Email,
                googleUser.DisplayName,
                googleUser.PictureUrl));
    }
}
