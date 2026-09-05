using System.Text;
using ClinicalAppointmentSystem.Application.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace ClinicalAppointmentSystem.Api.Authentication;

public static class JwtAuthenticationExtensions
{
    private const string Section = "Authentication:Jwt";

    public static JwtSettings ReadJwtSettings(this IConfiguration configuration)
    {
        var settings = new JwtSettings(
            Required(configuration, "Issuer"),
            Required(configuration, "Audience"),
            Required(configuration, "SigningKey"),
            configuration.GetValue($"{Section}:LifetimeMinutes", 60));

        settings.EnsureValid();
        return settings;
    }

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        JwtSettings settings)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = settings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(settings.SigningKey)),
                    ValidateLifetime = true,

                    ClockSkew = TimeSpan.FromSeconds(30),
                };
            });

        services.AddAuthorization(options =>
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());

        return services;
    }

    private static string Required(IConfiguration configuration, string key) =>
        configuration[$"{Section}:{key}"]
        ?? throw new InvalidOperationException(
            $"'{Section}:{key}' is not configured. "
            + "Set it in user-secrets (SigningKey) or appsettings (Issuer, Audience).");
}
