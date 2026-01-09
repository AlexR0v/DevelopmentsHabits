namespace DevHabit.Api.DTOs.Common;

public sealed record PaginationResult<T> : ICollectionResponse<T>
{
    public int TotalCount { get; init; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<T> Items { get; init; }
}
