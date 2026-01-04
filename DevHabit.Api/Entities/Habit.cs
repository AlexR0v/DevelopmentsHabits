namespace DevHabit.Api.Entities;

public sealed class Habit
{
    public string Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public HabitType Type { get; set; }
    public Frequency Frequency { get; set; }
    public Target Target { get; set; }
    public HabitStatus Status { get; set; }
    public bool IsArchived { get; set; }
    public DateOnly? EndDate { get; set; }
    public Milestone? Milestone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastCompletedAt { get; set; }

    public List<HabitTag> HabitTags { get; set; }
    public List<Tag> Tags { get; set; }
}

public enum HabitType
{
    None = 0,
    Binary = 1,
    Measurable = 2
}

public enum HabitStatus
{
    None = 0,
    Active = 1,
    Inactive = 2,
    Completed = 3
}

public class Frequency
{
    public FrequencyType Type { get; set; }
    public int TimesPerPeriod { get; set; }
}

public enum FrequencyType
{
    None = 0,
    Daily = 1,
    Weekly = 2,
    Monthly = 3
}

public class Target
{
    public int Value { get; set; }
    public string Unit { get; set; }
}

public class Milestone
{
    public int Target { get; set; }
    public int Current { get; set; }
}
