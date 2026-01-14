using Newtonsoft.Json;

namespace DevHabit.Api.DTOs.GitHub;

public sealed record GitHubEventDto(
    string Id,
    string Type,
    GitHubEventActorDto Actor,
    GitHubEventRepoDto Repo,
    GitHubEventPayloadDto Payload,
    bool Public,
    [JsonProperty("created_at")] DateTimeOffset CreatedAt
);

public sealed record GitHubEventActorDto(
    long Id,
    string Login,
    [JsonProperty("display_login")] 
    string DisplayLogin,
    [JsonProperty("gravatar_id")] 
    string GravatarId,
    Uri Url,
    [JsonProperty("avatar_url")] 
    Uri AvatarUrl);

public sealed record GitHubEventRepoDto(
    long Id,
    string Name,
    Uri Url);

public sealed record GitHubEventPayloadDto(
    string Action,
    ICollection<Commit>? Commits);

public record Commit(
    string Sha,
    Author Author,
    string Message,
    bool Distinct,
    Uri Url);

public record Author(
    string Email,
    string Name);
