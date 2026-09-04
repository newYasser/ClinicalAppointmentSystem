using ClinicalAppointmentSystem.Application.Common.Abstractions;
using ClinicalAppointmentSystem.Domain.Common;
using ClinicalAppointmentSystem.Domain.Entities;
using ClinicalAppointmentSystem.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ClinicalAppointmentSystem.Infrastructure.Persistence;

public class ClinicDbContext(DbContextOptions<ClinicDbContext> options)
    : DbContext(options), IClinicDbContext
{
    private const string DoctorSlotIndex = "UX_Appointments_Doctor_ActiveSlot";
    private const string PatientSlotIndex = "UX_Appointments_Patient_ActiveSlot";

    public DbSet<Specialty> Specialties => Set<Specialty>();

    public DbSet<Patient> Patients => Set<Patient>();

    public DbSet<Doctor> Doctors => Set<Doctor>();

    public DbSet<Appointment> Appointments => Set<Appointment>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampAuditFields();

        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            var conflict = TranslateSlotConflict(exception);

            if (conflict is not null)
            {
                throw conflict;
            }

            throw;
        }
    }

    public override int SaveChanges()
    {
        StampAuditFields();

        try
        {
            return base.SaveChanges();
        }
        catch (DbUpdateException exception)
        {
            var conflict = TranslateSlotConflict(exception);

            if (conflict is not null)
            {
                throw conflict;
            }

            throw;
        }
    }

    // The race-condition backstop. Two callers can both pass the service-level "is this slot
    // free?" check; the unique indexes are what actually prevent the double booking. The
    // provider-specific duplicate-key error is translated here because Application must not
    // reference Pomelo. The index names are therefore part of the contract.
    private static ConflictException? TranslateSlotConflict(DbUpdateException exception)
    {
        var message = exception.InnerException?.Message;

        if (string.IsNullOrEmpty(message))
        {
            return null;
        }

        if (message.Contains(DoctorSlotIndex, StringComparison.Ordinal))
        {
            return new ConflictException(
                ErrorCodes.DoctorSlotConflict,
                "That slot has just been taken for this doctor. Pick a free slot.");
        }

        if (message.Contains(PatientSlotIndex, StringComparison.Ordinal))
        {
            return new ConflictException(
                ErrorCodes.PatientSlotConflict,
                "That slot has just been taken for this patient. Pick a free slot.");
        }

        return null;
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
