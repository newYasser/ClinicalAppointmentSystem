using ClinicalAppointmentSystem.Application.Authentication;
using ClinicalAppointmentSystem.Application.Common.Abstractions;
using ClinicalAppointmentSystem.Infrastructure.Authentication;
using ClinicalAppointmentSystem.Infrastructure.Persistence;
using ClinicalAppointmentSystem.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicalAppointmentSystem.Infrastructure;

public static class DependencyInjection
{
    private static readonly MySqlServerVersion MySqlVersion =
        new(new Version(8, 4, 0));

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        string timeZoneId,
        JwtSettings jwtSettings,
        GoogleAuthSettings googleSettings)
    {
        services.AddDbContext<ClinicDbContext>(options =>
            options.UseMySql(connectionString, MySqlVersion));

        services.AddScoped<IClinicDbContext>(sp => sp.GetRequiredService<ClinicDbContext>());

        services.AddSingleton<IClinicClock>(new ClinicClock(TimeProvider.System, timeZoneId));

        services.AddSingleton<IAccessTokenIssuer>(sp =>
            new JwtAccessTokenIssuer(jwtSettings, sp.GetRequiredService<IClinicClock>()));

        services.AddSingleton<IGoogleTokenValidator>(new GoogleTokenValidator(googleSettings));

        return services;
    }
}
