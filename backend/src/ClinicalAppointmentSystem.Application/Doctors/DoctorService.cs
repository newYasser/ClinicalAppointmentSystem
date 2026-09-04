using ClinicalAppointmentSystem.Application.Common;
using ClinicalAppointmentSystem.Application.Common.Abstractions;
using ClinicalAppointmentSystem.Application.Common.Pagination;
using ClinicalAppointmentSystem.Domain.Common;
using ClinicalAppointmentSystem.Domain.Entities;
using ClinicalAppointmentSystem.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ClinicalAppointmentSystem.Application.Doctors;

public sealed class DoctorService(IClinicDbContext db) : IDoctorService
{
    private const string SortLastName = "lastName";
    private const string SortFirstName = "firstName";
    private const string SortSpecialty = "specialty";
    private const string SortAppointmentCount = "appointmentCount";

    public async Task<PagedResult<DoctorListItemDto>> GetListAsync(
        DoctorListQuery query,
        CancellationToken cancellationToken = default)
    {
        query.EnsureValid();

        var sortBy = query.ResolveSortBy(
            SortLastName,
            SortLastName,
            SortFirstName,
            SortSpecialty,
            SortAppointmentCount);

        if (query.SpecialtyId is not null && !string.IsNullOrWhiteSpace(query.Specialty))
        {
            throw DomainValidationException.ForField(
                "specialty",
                "Filter by either specialtyId or specialty, not both.");
        }

        var doctors = db.Doctors.AsNoTracking();

        var pattern = SearchTerm.ToLikePattern(query.Search);
        if (pattern is not null)
        {
            doctors = doctors.Where(d =>
                EF.Functions.Like(d.FirstName, pattern)
                || EF.Functions.Like(d.LastName, pattern)
                || EF.Functions.Like(d.FirstName + " " + d.LastName, pattern));
        }

        if (query.SpecialtyId is { } specialtyId)
        {
            doctors = doctors.Where(d => d.SpecialtyId == specialtyId);
        }
        else if (!string.IsNullOrWhiteSpace(query.Specialty))
        {
            var specialtyName = query.Specialty.Trim();
            doctors = doctors.Where(d => d.Specialty.Name == specialtyName);
        }

        var projected = Sort(doctors, sortBy, query.IsDescending)
            .Select(d => new DoctorListItemDto(
                d.Id,
                d.FirstName,
                d.LastName,
                "Dr. " + d.FirstName + " " + d.LastName,
                d.SpecialtyId,
                d.Specialty.Name,
                d.Appointments.Count));

        return await projected.ToPagedResultAsync(query.Page, query.PageSize, cancellationToken);
    }

    public async Task<DoctorDetailDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        await db.Doctors
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => new DoctorDetailDto(
                d.Id,
                d.FirstName,
                d.LastName,
                "Dr. " + d.FirstName + " " + d.LastName,
                d.SpecialtyId,
                d.Specialty.Name,
                d.Appointments.Count,
                d.CreatedAt,
                d.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken)
        ?? throw new NotFoundException(
            ErrorCodes.DoctorNotFound,
            $"Doctor {id} was not found.");

    public async Task<IReadOnlyList<DoctorLookupDto>> GetLookupAsync(
        DoctorLookupQuery query,
        CancellationToken cancellationToken = default)
    {
        var doctors = db.Doctors.AsNoTracking();

        var pattern = SearchTerm.ToLikePattern(query.Search);
        if (pattern is not null)
        {
            doctors = doctors.Where(d =>
                EF.Functions.Like(d.FirstName, pattern)
                || EF.Functions.Like(d.LastName, pattern)
                || EF.Functions.Like(d.FirstName + " " + d.LastName, pattern));
        }

        if (query.SpecialtyId is { } specialtyId)
        {
            doctors = doctors.Where(d => d.SpecialtyId == specialtyId);
        }

        return await doctors
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .ThenBy(d => d.Id)
            .Select(d => new DoctorLookupDto(
                d.Id,
                "Dr. " + d.FirstName + " " + d.LastName + " · " + d.Specialty.Name,
                d.SpecialtyId,
                d.Specialty.Name))
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<Doctor> Sort(
        IQueryable<Doctor> query,
        string sortBy,
        bool descending) =>
        (sortBy, descending) switch
        {
            (SortFirstName, false) => query.OrderBy(d => d.FirstName).ThenBy(d => d.LastName).ThenBy(d => d.Id),
            (SortFirstName, true) => query.OrderByDescending(d => d.FirstName).ThenByDescending(d => d.LastName).ThenBy(d => d.Id),
            (SortSpecialty, false) => query.OrderBy(d => d.Specialty.Name).ThenBy(d => d.LastName).ThenBy(d => d.Id),
            (SortSpecialty, true) => query.OrderByDescending(d => d.Specialty.Name).ThenBy(d => d.LastName).ThenBy(d => d.Id),
            (SortAppointmentCount, false) => query.OrderBy(d => d.Appointments.Count).ThenBy(d => d.Id),
            (SortAppointmentCount, true) => query.OrderByDescending(d => d.Appointments.Count).ThenBy(d => d.Id),
            (_, true) => query.OrderByDescending(d => d.LastName).ThenByDescending(d => d.FirstName).ThenBy(d => d.Id),
            _ => query.OrderBy(d => d.LastName).ThenBy(d => d.FirstName).ThenBy(d => d.Id),
        };
}
