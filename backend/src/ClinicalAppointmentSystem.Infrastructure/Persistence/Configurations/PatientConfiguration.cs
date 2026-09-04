using ClinicalAppointmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicalAppointmentSystem.Infrastructure.Persistence.Configurations;

internal sealed class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.DateOfBirth)
            .IsRequired();

        builder.Property(p => p.Phone)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Ignore(p => p.FullName);

        builder.HasIndex(p => new { p.LastName, p.FirstName })
            .HasDatabaseName("IX_Patients_LastName_FirstName");

        builder.HasIndex(p => p.Phone)
            .HasDatabaseName("IX_Patients_Phone");

        builder.HasIndex(p => p.Email)
            .HasDatabaseName("IX_Patients_Email");

        builder.HasData(SeedData.Patients);
    }
}
