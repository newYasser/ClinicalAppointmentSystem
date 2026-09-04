using ClinicalAppointmentSystem.Application.Common;
using ClinicalAppointmentSystem.Application.Common.Abstractions;
using ClinicalAppointmentSystem.Application.Common.Pagination;
using ClinicalAppointmentSystem.Domain.Entities;
using ClinicalAppointmentSystem.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ClinicalAppointmentSystem.Application.Appointments;

public sealed class AppointmentService(IClinicDbContext db, IClinicClock clock) : IAppointmentService
{
    private const string SortScheduledAt = "scheduledAt";
    private const string SortPatientName = "patientName";
    private const string SortDoctorName = "doctorName";
    private const string SortStatus = "status";

    public async Task<PagedResult<AppointmentListItemDto>> GetListAsync(
        AppointmentListQuery query,
        CancellationToken cancellationToken = default)
    {
        query.EnsureValid();

        var sortBy = query.ResolveSortBy(
            SortScheduledAt,
            SortScheduledAt,
            SortPatientName,
            SortDoctorName,
            SortStatus);

        if (query.Date is not null && (query.From is not null || query.To is not null))
        {
            throw DomainValidationException.ForField(
                "date",
                "Filter by either date or from/to, not both.");
        }

        if (query.From is { } fromCheck && query.To is { } toCheck && toCheck < fromCheck)
        {
            throw DomainValidationException.ForField("to", "to must be on or after from.");
        }

        var appointments = db.Appointments.AsNoTracking();

        if (query.Date is { } date)
        {
            var dayStart = date.ToDateTime(TimeOnly.MinValue);
            var nextDayStart = dayStart.AddDays(1);
            appointments = appointments.Where(a => a.ScheduledAt >= dayStart && a.ScheduledAt < nextDayStart);
        }
        else
        {
            if (query.From is { } from)
            {
                var fromStart = from.ToDateTime(TimeOnly.MinValue);
                appointments = appointments.Where(a => a.ScheduledAt >= fromStart);
            }

            if (query.To is { } to)
            {
                var afterTo = to.AddDays(1).ToDateTime(TimeOnly.MinValue);
                appointments = appointments.Where(a => a.ScheduledAt < afterTo);
            }
        }

        if (query.DoctorId is { } doctorId)
        {
            appointments = appointments.Where(a => a.DoctorId == doctorId);
        }

        if (query.PatientId is { } patientId)
        {
            appointments = appointments.Where(a => a.PatientId == patientId);
        }

        if (query.Status is { } status)
        {
            appointments = appointments.Where(a => a.Status == status);
        }

        var pattern = SearchTerm.ToLikePattern(query.Search);
        if (pattern is not null)
        {
            appointments = appointments.Where(a =>
                EF.Functions.Like(a.Patient.FirstName, pattern)
                || EF.Functions.Like(a.Patient.LastName, pattern)
                || EF.Functions.Like(a.Patient.FirstName + " " + a.Patient.LastName, pattern)
                || EF.Functions.Like(a.Doctor.FirstName, pattern)
                || EF.Functions.Like(a.Doctor.LastName, pattern)
                || EF.Functions.Like(a.Doctor.FirstName + " " + a.Doctor.LastName, pattern));
        }

        var page = await Sort(appointments, sortBy, query.IsDescending)
            .ToRows()
            .ToPagedResultAsync(query.Page, query.PageSize, cancellationToken);

        var nowLocal = clock.NowLocal;
        return page.Map(row => row.ToDto(nowLocal));
    }

    private static IQueryable<Appointment> Sort(
        IQueryable<Appointment> query,
        string sortBy,
        bool descending) =>
        (sortBy, descending) switch
        {
            (SortPatientName, false) => query.OrderBy(a => a.Patient.LastName).ThenBy(a => a.Patient.FirstName).ThenBy(a => a.Id),
            (SortPatientName, true) => query.OrderByDescending(a => a.Patient.LastName).ThenByDescending(a => a.Patient.FirstName).ThenBy(a => a.Id),
            (SortDoctorName, false) => query.OrderBy(a => a.Doctor.LastName).ThenBy(a => a.Doctor.FirstName).ThenBy(a => a.Id),
            (SortDoctorName, true) => query.OrderByDescending(a => a.Doctor.LastName).ThenByDescending(a => a.Doctor.FirstName).ThenBy(a => a.Id),
            (SortStatus, false) => query.OrderBy(a => a.Status).ThenBy(a => a.ScheduledAt).ThenBy(a => a.Id),
            (SortStatus, true) => query.OrderByDescending(a => a.Status).ThenBy(a => a.ScheduledAt).ThenBy(a => a.Id),
            (_, true) => query.OrderByDescending(a => a.ScheduledAt).ThenBy(a => a.Id),
            _ => query.OrderBy(a => a.ScheduledAt).ThenBy(a => a.Id),
        };
}
