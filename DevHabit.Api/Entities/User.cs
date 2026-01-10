namespace DevHabit.Api.Entities;

public sealed class User
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string IdentityId { get; set; }

    public static string CreateNewId() => $"u_{Guid.CreateVersion7()}";
}
