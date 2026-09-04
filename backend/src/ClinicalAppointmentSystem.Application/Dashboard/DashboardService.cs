using ClinicalAppointmentSystem.Application.Common.Abstractions;
using ClinicalAppointmentSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClinicalAppointmentSystem.Application.Dashboard;

public sealed class DashboardService(IClinicDbContext db, IClinicClock clock) : IDashboardService
{
    public async Task<DashboardSummaryDto> GetSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var today = clock.Today;
        var dayStart = today.ToDateTime(TimeOnly.MinValue);
        var nextDayStart = dayStart.AddDays(1);

        var totalPatients = await db.Patients.CountAsync(cancellationToken);
        var totalDoctors = await db.Doctors.CountAsync(cancellationToken);
        var specialtyCount = await db.Specialties.CountAsync(cancellationToken);

        var upcomingAppointmentCount = await db.Appointments.CountAsync(
            a => a.Status == AppointmentStatus.Scheduled && a.ScheduledAt >= nextDayStart,
            cancellationToken);

        var rows = await db.Appointments
            .AsNoTracking()
            .Where(a => a.ScheduledAt >= dayStart && a.ScheduledAt < nextDayStart)
            .OrderBy(a => a.ScheduledAt)
            .ThenBy(a => a.Id)
            .Select(a => new
            {
                a.Id,
                a.ScheduledAt,
                a.Status,
                a.PatientId,
                PatientFirstName = a.Patient.FirstName,
                PatientLastName = a.Patient.LastName,
                a.DoctorId,
                DoctorFirstName = a.Doctor.FirstName,
                DoctorLastName = a.Doctor.LastName,
                Specialty = a.Doctor.Specialty.Name,
            })
            .ToListAsync(cancellationToken);

        var todaySchedule = rows
            .Select(row => new DashboardAppointmentDto(
                row.Id,
                TimeOnly.FromDateTime(row.ScheduledAt),
                row.PatientId,
                $"{row.PatientFirstName} {row.PatientLastName}",
                row.DoctorId,
                $"Dr. {row.DoctorFirstName} {row.DoctorLastName}",
                row.Specialty,
                row.Status))
            .ToList();

        return new DashboardSummaryDto(
            totalPatients,
            totalDoctors,
            todaySchedule.Count,
            upcomingAppointmentCount,
            specialtyCount,
            today,
            todaySchedule);
    }
}
