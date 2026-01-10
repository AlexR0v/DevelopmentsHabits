using DevHabit.Api.DTOs.Auth;
using DevHabit.Api.Entities;

namespace DevHabit.Api.DTOs.Users;

public static class UserMappings
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public static User ToEntity(this RegisterUserDto dto)
    {
        return new()
        {
            Id = User.CreateNewId(),
            Name = dto.Name,
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public static void UpdateFromDto(this User user, UpdateUserDto dto)
    {
        user.Name = dto.Name;
        user.UpdatedAt = DateTime.UtcNow;
    }
}
