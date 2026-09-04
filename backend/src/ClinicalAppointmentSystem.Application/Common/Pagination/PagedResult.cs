namespace ClinicalAppointmentSystem.Application.Common.Pagination;

public sealed record PagedResult<T>
{
    public required IReadOnlyList<T> Items { get; init; }

    public required int Page { get; init; }

    public required int PageSize { get; init; }

    public required int TotalCount { get; init; }

    public int TotalPages => TotalCount == 0 ? 1 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    public int From => TotalCount == 0 ? 0 : ((Page - 1) * PageSize) + 1;

    public int To => TotalCount == 0 ? 0 : Math.Min(Page * PageSize, TotalCount);

    public static PagedResult<T> Empty(int pageSize) =>
        new() { Items = [], Page = 1, PageSize = pageSize, TotalCount = 0 };

    public PagedResult<TOut> Map<TOut>(Func<T, TOut> selector) =>
        new()
        {
            Items = [.. Items.Select(selector)],
            Page = Page,
            PageSize = PageSize,
            TotalCount = TotalCount,
        };
}
