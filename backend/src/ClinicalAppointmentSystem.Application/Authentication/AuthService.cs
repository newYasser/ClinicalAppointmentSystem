namespace ClinicalAppointmentSystem.Application.Authentication;

public sealed class AuthService(
    IGoogleTokenValidator googleTokens,
    IAccessTokenIssuer accessTokens) : IAuthService
{
    private const string BearerTokenType = "Bearer";

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
