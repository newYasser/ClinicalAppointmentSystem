using ClinicalAppointmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicalAppointmentSystem.Infrastructure.Persistence.Configurations;

internal sealed class SpecialtyConfiguration : IEntityTypeConfiguration<Specialty>
{
    public void Configure(EntityTypeBuilder<Specialty> builder)
    {
        builder.ToTable("Specialties");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(s => s.Name)
            .IsUnique()
            .HasDatabaseName("UX_Specialties_Name");

        builder.HasData(SeedData.Specialties);
    }
}
