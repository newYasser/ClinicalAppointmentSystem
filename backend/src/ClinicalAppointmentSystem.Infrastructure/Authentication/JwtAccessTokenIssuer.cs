using System.Text;
using ClinicalAppointmentSystem.Application.Authentication;
using ClinicalAppointmentSystem.Application.Common.Abstractions;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ClinicalAppointmentSystem.Infrastructure.Authentication;

public sealed class JwtAccessTokenIssuer : IAccessTokenIssuer
{
    private readonly JwtSettings settings;
    private readonly IClinicClock clock;
    private readonly SigningCredentials credentials;
    private readonly JsonWebTokenHandler handler = new();

    public JwtAccessTokenIssuer(JwtSettings settings, IClinicClock clock)
    {
        this.settings = settings;
        this.clock = clock;

        credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SigningKey)),
            SecurityAlgorithms.HmacSha256);
    }

    public AccessToken Issue(TokenSubject subject)
    {
        // Token timestamps are UTC: 'exp' and 'nbf' are defined as UTC epoch seconds, and
        // the clinic's local time zone has no bearing on them. IClinicClock is used rather
        // than DateTime.UtcNow so expiry stays controllable from tests.
        var issuedAt = clock.UtcNow;
        var expiresAt = issuedAt.AddMinutes(settings.LifetimeMinutes);

        var claims = new Dictionary<string, object>
        {
            [JwtRegisteredClaimNames.Sub] = subject.Subject,
            [JwtRegisteredClaimNames.Email] = subject.Email,
            [JwtRegisteredClaimNames.Name] = subject.DisplayName,
            [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString("N"),
        };

        if (subject.PictureUrl is { Length: > 0 } pictureUrl)
        {
            claims["picture"] = pictureUrl;
        }

        var token = handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = settings.Issuer,
            Audience = settings.Audience,
            IssuedAt = issuedAt,
            NotBefore = issuedAt,
            Expires = expiresAt,
            Claims = claims,
            SigningCredentials = credentials,
        });

        return new AccessToken(
            token,
            expiresAt,
            (int)(expiresAt - issuedAt).TotalSeconds);
    }
}
