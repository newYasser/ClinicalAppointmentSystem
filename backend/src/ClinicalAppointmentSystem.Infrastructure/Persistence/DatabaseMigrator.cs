using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicalAppointmentSystem.Infrastructure.Persistence;

public static class DatabaseMigrator
{
    /// Applies any pending migrations, including the seed rows they carry.
    /// Intended for environments that have no other way to run
    public static async Task MigrateClinicDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();

        await scope.ServiceProvider
            .GetRequiredService<ClinicDbContext>()
            .Database
            .MigrateAsync(cancellationToken);
    }
}
