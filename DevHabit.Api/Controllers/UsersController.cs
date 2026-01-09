using DevHabit.Api.Database;
using DevHabit.Api.DTOs.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;

[ApiController]
[Route("users")]
public sealed class UsersController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserByIdAsync(string id)
    {
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
}
