namespace DevHabit.Api.DTOs.Habits;

public record HabitShortDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public DateTime? LastCompletedAtUtc { get; init; }
    public string[] Tags { get; init; } = [];
}
