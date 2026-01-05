namespace DevHabit.Api.DTOs.Common;

public sealed record PaginationResult<T> : ICollectionResponse<T>
{
    public int TotalCount { get; init; }
    public List<T> Items { get; init; }
}
