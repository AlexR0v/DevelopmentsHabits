using DevHabit.Api.Database;
using DevHabit.Api.DTOs.Auth;
using DevHabit.Api.DTOs.Users;
using DevHabit.Api.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DevHabit.Api.Controllers;

[ApiController]
[Route("auth")]
[AllowAnonymous]
public sealed class AuthController(
    ApplicationIdentityDbContext identityDbContext,
    ApplicationDbContext dbContext,
    UserManager<IdentityUser> userManager,
    TokenProvider tokenProvider
) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(
        [FromBody] RegisterUserDto registerUserDto,
        IValidator<RegisterUserDto> validator
    )
    {
        await using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync();
        dbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await dbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

        await validator.ValidateAndThrowAsync(registerUserDto);

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
                statusCode: StatusCodes.Status409Conflict
            );
        }

        var identityUser = new IdentityUser
        {
            UserName = registerUserDto.Name,
            Email = registerUserDto.Email
        };
        IdentityResult createUserResult = await userManager.CreateAsync(identityUser, registerUserDto.Password);
        if (!createUserResult.Succeeded)
        {
            return Problem(
                detail: "Failed to create user",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: new Dictionary<string, object?>
                {
                    { "errors", createUserResult.Errors.ToDictionary(error => error.Code, error => error.Description) }
                }
            );
        }

        Entities.User user = registerUserDto.ToEntity();
        user.IdentityId = identityUser.Id;

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        await transaction.CommitAsync();

        AccessTokensDto accessToken = tokenProvider.Create(new TokenRequestDto
            { UserId = identityUser.Id, Email = identityUser.Email });

        return Ok(accessToken);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginUserDto loginUserDto,
        IValidator<LoginUserDto> validator
    )
    {
        await validator.ValidateAndThrowAsync(loginUserDto);
        IdentityUser? identityUser = await userManager.FindByEmailAsync(loginUserDto.Email);
        if (identityUser is null || !await userManager.CheckPasswordAsync(identityUser, loginUserDto.Password))
        {
            return Unauthorized();
        }

        AccessTokensDto accessToken = tokenProvider.Create(new TokenRequestDto
            { UserId = identityUser.Id, Email = identityUser.Email! });

        return Ok(accessToken);
    }
}
