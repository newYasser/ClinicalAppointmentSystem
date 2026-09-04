using ClinicalAppointmentSystem.Application.Common;
using ClinicalAppointmentSystem.Application.Common.Abstractions;
using ClinicalAppointmentSystem.Application.Common.Pagination;
using ClinicalAppointmentSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicalAppointmentSystem.Application.Patients;

public sealed class PatientService(IClinicDbContext db) : IPatientService
{
    private const string SortLastName = "lastName";
    private const string SortFirstName = "firstName";
    private const string SortDateOfBirth = "dateOfBirth";
    private const string SortAppointmentCount = "appointmentCount";

    public async Task<PagedResult<PatientListItemDto>> GetListAsync(
        PatientListQuery query,
        CancellationToken cancellationToken = default)
    {
        query.EnsureValid();

        var sortBy = query.ResolveSortBy(
            SortLastName,
            SortLastName,
            SortFirstName,
            SortDateOfBirth,
            SortAppointmentCount);

        var patients = db.Patients.AsNoTracking();

        var pattern = SearchTerm.ToLikePattern(query.Search);
        if (pattern is not null)
        {
            patients = patients.Where(p =>
                EF.Functions.Like(p.FirstName, pattern)
                || EF.Functions.Like(p.LastName, pattern)
                || EF.Functions.Like(p.FirstName + " " + p.LastName, pattern)
                || EF.Functions.Like(p.Phone, pattern)
                || EF.Functions.Like(p.Email, pattern));
        }


        var projected = Sort(patients, sortBy, query.IsDescending)
            .Select(p => new PatientListItemDto(
                p.Id,
                p.FirstName,
                p.LastName,
                p.FirstName + " " + p.LastName,
                p.DateOfBirth,
                p.Phone,
                p.Email,
                p.Appointments.Count));

        return await projected.ToPagedResultAsync(query.Page, query.PageSize, cancellationToken);
    }

    private static IQueryable<Patient> Sort(
        IQueryable<Patient> query,
        string sortBy,
        bool descending) =>
        (sortBy, descending) switch
        {
            (SortFirstName, false) => query.OrderBy(p => p.FirstName).ThenBy(p => p.LastName),
            (SortFirstName, true) => query.OrderByDescending(p => p.FirstName).ThenByDescending(p => p.LastName),
            (SortDateOfBirth, false) => query.OrderBy(p => p.DateOfBirth).ThenBy(p => p.Id),
            (SortDateOfBirth, true) => query.OrderByDescending(p => p.DateOfBirth).ThenBy(p => p.Id),
            (SortAppointmentCount, false) => query.OrderBy(p => p.Appointments.Count).ThenBy(p => p.Id),
            (SortAppointmentCount, true) => query.OrderByDescending(p => p.Appointments.Count).ThenBy(p => p.Id),
            (_, true) => query.OrderByDescending(p => p.LastName).ThenByDescending(p => p.FirstName),
            _ => query.OrderBy(p => p.LastName).ThenBy(p => p.FirstName),
        };
}
