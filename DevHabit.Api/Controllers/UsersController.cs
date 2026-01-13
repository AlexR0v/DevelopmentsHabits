using System.Net.Mime;
using System.Security.Claims;
using DevHabit.Api.Database;
using DevHabit.Api.DTOs.Users;
using DevHabit.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;

[Authorize]
[ApiController]
[Route("users")]
public sealed class UsersController(ApplicationDbContext dbContext, UserContext userContext) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserByIdAsync(string id)
    {
        string? userId = await userContext.GetUserIdAsync();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        if (id != userId)
        {
            return Forbid();
        }

        UserDto? userDto = await dbContext.Users
            .Where(u => u.Id == id)
            .Select(u => u.ToDto())
            .FirstOrDefaultAsync();
        if (userDto is null)
        {
            return NotFound();
        }

        return Ok(userDto);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        string? userId = await userContext.GetUserIdAsync();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        UserDto? user = await dbContext.Users.AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => x.ToDto())
            .FirstOrDefaultAsync();

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}
