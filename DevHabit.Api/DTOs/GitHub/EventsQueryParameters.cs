using DevHabit.Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DevHabit.Api.DTOs.GitHub;

public sealed record EventsQueryParameters
{
    [FromQuery]
    public int Page { get; init; } = 1;
    [FromQuery]
    public int PageSize { get; init; } = 10;
    [FromQuery]
    public string UserName { get; init; }
}
