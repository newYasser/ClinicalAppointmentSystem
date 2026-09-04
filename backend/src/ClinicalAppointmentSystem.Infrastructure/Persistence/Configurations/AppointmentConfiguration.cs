using ClinicalAppointmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicalAppointmentSystem.Infrastructure.Persistence.Configurations;

internal sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.ScheduledAt)
            .IsRequired();

        builder.Property(a => a.DurationMinutes)
            .IsRequired()
            .HasColumnType("smallint");

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.Notes)
            .HasMaxLength(1000);

        builder.Ignore(a => a.Date);
        builder.Ignore(a => a.StartTime);
        builder.Ignore(a => a.EndTime);
        builder.Ignore(a => a.IsLive);
        builder.Ignore(a => a.CanCancel);
        builder.Ignore(a => a.CanComplete);

        builder.HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.DoctorId, a.ActiveSlot })
            .IsUnique()
            .HasDatabaseName("UX_Appointments_Doctor_ActiveSlot");

        builder.HasIndex(a => new { a.PatientId, a.ActiveSlot })
            .IsUnique()
            .HasDatabaseName("UX_Appointments_Patient_ActiveSlot");

        builder.HasIndex(a => a.ScheduledAt)
            .HasDatabaseName("IX_Appointments_ScheduledAt");
    }
}
