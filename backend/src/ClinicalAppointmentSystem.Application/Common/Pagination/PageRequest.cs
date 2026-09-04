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
}
