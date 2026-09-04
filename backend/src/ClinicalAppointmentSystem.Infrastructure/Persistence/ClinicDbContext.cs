using ClinicalAppointmentSystem.Application.Common.Abstractions;
using ClinicalAppointmentSystem.Domain.Common;
using ClinicalAppointmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicalAppointmentSystem.Infrastructure.Persistence;

public class ClinicDbContext(DbContextOptions<ClinicDbContext> options)
    : DbContext(options), IClinicDbContext
{
    public DbSet<Specialty> Specialties => Set<Specialty>();

    public DbSet<Patient> Patients => Set<Patient>();

    public DbSet<Doctor> Doctors => Set<Doctor>();

    public DbSet<Appointment> Appointments => Set<Appointment>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        StampAuditFields();
        return base.SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClinicDbContext).Assembly);

    
    private void StampAuditFields()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
    }
}
