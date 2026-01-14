using DevHabit.Api.Database;
using DevHabit.Api.DTOs.Auth;
using DevHabit.Api.DTOs.Users;
using DevHabit.Api.Entities;
using DevHabit.Api.Services;
using DevHabit.Api.Settings;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace DevHabit.Api.Controllers;

[ApiController]
[Route("auth")]
[AllowAnonymous]
public sealed class AuthController(
    ApplicationIdentityDbContext identityDbContext,
    ApplicationDbContext dbContext,
    UserManager<IdentityUser> userManager,
    TokenProvider tokenProvider,
    IOptions<JwtAuthOptions> jwtAuthOptions
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

        IdentityResult createRoleResult = await userManager.AddToRoleAsync(identityUser, Roles.Member);
        if (!createRoleResult.Succeeded)
        {
            return Problem(
                detail: "Failed to create role",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: new Dictionary<string, object?>
                {
                    { "errors", createRoleResult.Errors.ToDictionary(error => error.Code, error => error.Description) }
                }
            );
        }

        Entities.User user = registerUserDto.ToEntity();
        user.IdentityId = identityUser.Id;

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        AccessTokensDto accessToken = tokenProvider.Create(new TokenRequestDto
            { UserId = identityUser.Id, Email = identityUser.Email, Roles = [Roles.Member] });

        RefreshToken refreshToken = new()
        {
            Id = Guid.CreateVersion7(),
            UserId = identityUser.Id,
            Token = accessToken.RefreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(jwtAuthOptions.Value.RefreshTokenExpirationInDays),
        };

        identityDbContext.RefreshTokens.Add(refreshToken);
        await identityDbContext.SaveChangesAsync();

        await transaction.CommitAsync();

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

        IList<string> roles = await userManager.GetRolesAsync(identityUser);

        AccessTokensDto accessToken = tokenProvider.Create(new TokenRequestDto
            { UserId = identityUser.Id, Email = identityUser.Email!, Roles = roles });

        RefreshToken refreshToken = new()
        {
            Id = Guid.CreateVersion7(),
            UserId = identityUser.Id,
            Token = accessToken.RefreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(jwtAuthOptions.Value.RefreshTokenExpirationInDays),
        };

        identityDbContext.RefreshTokens.Add(refreshToken);
        await identityDbContext.SaveChangesAsync();

        return Ok(accessToken);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        RefreshTokenDto refreshTokenDto,
        IValidator<RefreshTokenDto> validator
    )
    {
        await validator.ValidateAndThrowAsync(refreshTokenDto);

        RefreshToken? refreshToken = await identityDbContext.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == refreshTokenDto.RefreshToken);

        if (refreshToken is null || refreshToken.ExpiresAtUtc < DateTime.UtcNow)
        {
            return Unauthorized();
        }

        IList<string> roles = await userManager.GetRolesAsync(refreshToken.User);

        TokenRequestDto tokenRequest = new()
        {
            UserId = refreshToken.UserId,
            Email = refreshToken.User.Email!,
            Roles = roles,
        };

        AccessTokensDto accessTokens = tokenProvider.Create(tokenRequest);

        refreshToken.Token = accessTokens.RefreshToken;
        refreshToken.ExpiresAtUtc = DateTime.UtcNow.AddMinutes(jwtAuthOptions.Value.RefreshTokenExpirationInDays);

        await identityDbContext.SaveChangesAsync();

        return Ok(accessTokens);
    }
}
