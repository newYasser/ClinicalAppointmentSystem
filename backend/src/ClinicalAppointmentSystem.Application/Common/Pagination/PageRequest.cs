using ClinicalAppointmentSystem.Domain.Exceptions;

namespace ClinicalAppointmentSystem.Application.Common.Pagination;

public class PageRequest
{
    public const int DefaultPageSize = 10;

    public static readonly int[] AllowedPageSizes = [10, 25, 50];

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = DefaultPageSize;

    public string? SortBy { get; set; }

    public string? SortDir { get; set; }

    public bool IsDescending =>
        string.Equals(SortDir, "desc", StringComparison.OrdinalIgnoreCase);

    public virtual void EnsureValid()
    {
        if (Page < 1)
        {
            throw DomainValidationException.ForField("Page", "page must be 1 or greater.");
        }

        if (!AllowedPageSizes.Contains(PageSize))
        {
            throw DomainValidationException.ForField(
                "PageSize",
                $"pageSize must be one of {string.Join(", ", AllowedPageSizes)}.");
        }

        if (SortDir is not null
            && !string.Equals(SortDir, "asc", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(SortDir, "desc", StringComparison.OrdinalIgnoreCase))
        {
            throw DomainValidationException.ForField("SortDir", "sortDir must be asc or desc.");
        }
    }

    public string ResolveSortBy(string defaultColumn, params string[] allowed)
    {
        if (string.IsNullOrWhiteSpace(SortBy))
        {
            return defaultColumn;
        }

        var requested = SortBy.Trim();

        var match = allowed.FirstOrDefault(
            column => string.Equals(column, requested, StringComparison.OrdinalIgnoreCase));

        return match ?? throw DomainValidationException.ForField(
            "SortBy",
            $"sortBy must be one of {string.Join(", ", allowed)}.");
    }
}
