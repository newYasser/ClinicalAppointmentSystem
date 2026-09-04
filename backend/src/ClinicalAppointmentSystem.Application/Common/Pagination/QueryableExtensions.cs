using Microsoft.EntityFrameworkCore;

namespace ClinicalAppointmentSystem.Application.Common.Pagination;

public static class QueryableExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);

        if (totalCount == 0)
        {
            return PagedResult<T>.Empty(pageSize);
        }

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var currentPage = Math.Clamp(page, 1, totalPages);

        var items = await query
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>
        {
            Items = items,
            Page = currentPage,
            PageSize = pageSize,
            TotalCount = totalCount,
        };
    }
}
