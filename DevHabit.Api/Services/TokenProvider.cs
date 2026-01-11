using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DevHabit.Api.DTOs.Auth;
using DevHabit.Api.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace DevHabit.Api.Services;

public sealed class TokenProvider(IOptions<JwtAuthOptions> jwtAuthOptions)
{
    public AccessTokensDto Create(TokenRequestDto tokenRequest)
    {
        return new(GenerateToken(tokenRequest), GenerateRefreshToken());
    }

    private string GenerateToken(TokenRequestDto tokenRequest)
    {
        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(jwtAuthOptions.Value.Key));
        SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, tokenRequest.UserId),
            new(JwtRegisteredClaimNames.Email, tokenRequest.Email),
            //.. tokenRequest.Roles.Select(role => new Claim(JwtCustomClaimNames.Role, role)),
        ];

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Issuer = jwtAuthOptions.Value.Issuer,
            Audience = jwtAuthOptions.Value.Audience,
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = signingCredentials,
            Expires = DateTime.UtcNow.AddMinutes(jwtAuthOptions.Value.ExpirationInMinutes),
        };

        JsonWebTokenHandler handler = new();
        string accessToken = handler.CreateToken(tokenDescriptor);

        return accessToken;
    }

    private static string GenerateRefreshToken()
    {
        byte[] guidBytes = Encoding.UTF8.GetBytes(Guid.CreateVersion7().ToString());
        byte[] randomBytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String([.. guidBytes, .. randomBytes]);
    }
}
