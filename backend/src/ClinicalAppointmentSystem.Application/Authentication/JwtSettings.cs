namespace ClinicalAppointmentSystem.Application.Authentication;

public sealed record JwtSettings(
    string Issuer,
    string Audience,
    string SigningKey,
    int LifetimeMinutes)
{
    public const int MinimumSigningKeyBytes = 32;

    public void EnsureValid()
    {
        if (System.Text.Encoding.UTF8.GetByteCount(SigningKey) < MinimumSigningKeyBytes)
        {
            throw new InvalidOperationException(
                $"'Authentication:Jwt:SigningKey' must be at least {MinimumSigningKeyBytes} bytes. "
                + "Generate one with: openssl rand -base64 48");
        }

        if (LifetimeMinutes < 1)
        {
            throw new InvalidOperationException(
                "'Authentication:Jwt:LifetimeMinutes' must be 1 or greater.");
        }
    }
}
