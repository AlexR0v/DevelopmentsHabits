namespace DevHabit.Api.DTOs.Users;

public sealed record UpdateUserDto
{
    public required string Name { get; set; }
}
