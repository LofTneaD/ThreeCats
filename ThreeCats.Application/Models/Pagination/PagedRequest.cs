namespace ThreeCats.Application.Models.Pagination;

public sealed record PagedRequest
{
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = DefaultPageSize;

    public int NormalizedPageNumber => Math.Max(PageNumber, 1);

    public int NormalizedPageSize =>
        Math.Clamp(PageSize, 1, MaxPageSize);

    public int Skip =>
        (NormalizedPageNumber - 1) * NormalizedPageSize;
}
