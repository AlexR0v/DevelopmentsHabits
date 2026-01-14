using DevHabit.Api.DTOs.Common;
using DevHabit.Api.DTOs.GitHub;
using DevHabit.Api.DTOs.Habits;
using DevHabit.Api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHabit.Api.Controllers;

[Authorize]
[ApiController]
[Route("github")]
public sealed class GitHubController(
    GitHubAccessTokenService gitHubAccessTokenService,
    GitHubService gitHubService,
    UserContext userContext) : ControllerBase
{
    [HttpGet("profile")]
    public async Task<IActionResult> GetUserProfile(CancellationToken cancellationToken)
    {
        string? userId = await userContext.GetUserIdAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        string? accessToken = await gitHubAccessTokenService.GetAsync(userId, cancellationToken);

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return NotFound();
        }

        GitHubUserProfileDto? userProfile = await gitHubService.GetUserProfileAsync(accessToken, cancellationToken);

        if (userProfile is null)
        {
            return NotFound();
        }

        return Ok(userProfile);
    }

    [HttpGet("events")]
    public async Task<IActionResult> GetUserEvents(EventsQueryParameters query, CancellationToken cancellationToken)
    {
        string? userId = await userContext.GetUserIdAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        string? accessToken = await gitHubAccessTokenService.GetAsync(userId, cancellationToken);

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return NotFound();
        }

        IReadOnlyList<GitHubEventDto>? events =
            await gitHubService.GetUserEventsAsync(query.UserName, accessToken, query.Page, query.PageSize,
                cancellationToken);
        
        var result = new PaginationResult<GitHubEventDto>
        {
            Items = (List<GitHubEventDto>)events,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = 0
        };
        return Ok(result);
    }

    [HttpPut("personal-access-token")]
    public async Task<IActionResult> StoreAccessToken(
        StoreGithubAccessTokenDto storeGithubAccessTokenDto,
        IValidator<StoreGithubAccessTokenDto> validator,
        CancellationToken cancellationToken)
    {
        string? userId = await userContext.GetUserIdAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        await validator.ValidateAndThrowAsync(storeGithubAccessTokenDto, cancellationToken);

        await gitHubAccessTokenService.StoreAsync(userId, storeGithubAccessTokenDto, cancellationToken);

        return NoContent();
    }

    [HttpDelete("personal-access-token")]
    public async Task<IActionResult> RevokeAccessToken(CancellationToken cancellationToken)
    {
        string? userId = await userContext.GetUserIdAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        await gitHubAccessTokenService.RevokeAsync(userId, cancellationToken);

        return NoContent();
    }
}
