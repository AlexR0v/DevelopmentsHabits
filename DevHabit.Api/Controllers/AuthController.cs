using DevHabit.Api.Database;
using DevHabit.Api.DTOs.Auth;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DevHabit.Api.Controllers;

[ApiController]
[Route("auth")]
[AllowAnonymous]
public sealed class AuthController(
    ApplicationIdentityDbContext identityDbContext,
    ApplicationDbContext dbContext,
    UserManager<IdentityUser> userManager
) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(
        [FromBody] RegisterUserDto registerUserDto,
        IValidator<RegisterUserDto> validator,
        ProblemDetailsFactory problemDetailsFactory
    )
    {
        ValidationResult validationResult = await validator.ValidateAsync(registerUserDto);

        if (!validationResult.IsValid)
        {
            ProblemDetails problem = problemDetailsFactory.CreateProblemDetails(
                HttpContext,
                StatusCodes.Status400BadRequest);
            problem.Extensions.Add("errors", validationResult.ToDictionary());

            return BadRequest(problem);
        }

        bool emailIsTaken = await userManager.FindByEmailAsync(registerUserDto.Email) is not null;

        if (emailIsTaken)
        {
            return Problem(
                detail: $"Email '{registerUserDto.Email}' is already taken",
                statusCode: StatusCodes.Status409Conflict);
        }

        bool usernameIsTaken = await userManager.FindByNameAsync(registerUserDto.Name) is not null;

        if (usernameIsTaken)
        {
            return Problem(
                detail: $"Username '{registerUserDto.Name}' is already taken",
                statusCode: StatusCodes.Status409Conflict);
        }

        return Ok("accesstoken");
    }
}
