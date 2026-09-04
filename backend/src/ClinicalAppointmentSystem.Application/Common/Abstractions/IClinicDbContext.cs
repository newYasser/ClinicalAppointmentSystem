using ClinicalAppointmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicalAppointmentSystem.Application.Common.Abstractions;

public interface IClinicDbContext
{
    DbSet<Specialty> Specialties { get; }

    DbSet<Patient> Patients { get; }

    DbSet<Doctor> Doctors { get; }

    DbSet<Appointment> Appointments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
