using ClinicalAppointmentSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicalAppointmentSystem.Infrastructure;

public static class DependencyInjection
{
    private static readonly MySqlServerVersion MySqlVersion =
        new(new Version(8, 4, 0));

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<ClinicDbContext>(options =>
            options.UseMySql(connectionString, MySqlVersion));

        return services;
    }
}