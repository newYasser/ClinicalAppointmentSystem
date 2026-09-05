using ClinicalAppointmentSystem.Application.Authentication;

namespace ClinicalAppointmentSystem.Api.Authentication;

public static class GoogleAuthenticationExtensions
{
    private const string Section = "Authentication:Google";

    public static GoogleAuthSettings ReadGoogleSettings(this IConfiguration configuration) =>
        new(configuration[$"{Section}:ClientId"]
            ?? throw new InvalidOperationException(
                $"'{Section}:ClientId' is not configured. Set it with: "
                + "dotnet user-secrets set \"Authentication:Google:ClientId\" \"<id>.apps.googleusercontent.com\""));
}
