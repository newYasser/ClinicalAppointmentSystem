using ClinicalAppointmentSystem.Application.Authentication;
using ClinicalAppointmentSystem.Domain.Common;
using ClinicalAppointmentSystem.Domain.Exceptions;
using Google.Apis.Auth;

namespace ClinicalAppointmentSystem.Infrastructure.Authentication;

public sealed class GoogleTokenValidator(GoogleAuthSettings settings) : IGoogleTokenValidator
{
    // ValidateAsync checks the signature against Google's published keys and verifies
    // issuer, expiry and audience. Audience is what ties a token to this application:
    // without it any Google-issued token for any app would be accepted.
    private readonly GoogleJsonWebSignature.ValidationSettings validationSettings = new()
    {
        Audience = [settings.ClientId],
    };

    public async Task<GoogleUser> ValidateAsync(
        string idToken,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // ValidateAsync throws ArgumentException rather than InvalidJwtException on an
        // empty token, which would surface as a 500 instead of a 401.
        if (string.IsNullOrWhiteSpace(idToken))
        {
            throw new UnauthorizedException(
                ErrorCodes.InvalidGoogleToken,
                "That Google sign-in could not be verified. Please sign in again.");
        }

        GoogleJsonWebSignature.Payload payload;

        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings);
        }
        catch (InvalidJwtException exception)
        {
            throw new UnauthorizedException(
                ErrorCodes.InvalidGoogleToken,
                "That Google sign-in could not be verified. Please sign in again.")
                .With("reason", exception.Message);
        }

        return new GoogleUser(
            payload.Subject,
            payload.Email,
            payload.EmailVerified,
            payload.Name,
            payload.Picture);
    }
}
