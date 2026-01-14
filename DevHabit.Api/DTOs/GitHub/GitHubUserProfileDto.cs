using Newtonsoft.Json;

namespace DevHabit.Api.DTOs.GitHub;

public sealed class GitHubUserProfileDto
{
    [JsonProperty("login")]
    public string Login { get; set; } = default!;

    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("user_view_type")]
    public string? UserViewType { get; set; }

    [JsonProperty("node_id")]
    public string NodeId { get; set; } = default!;

    [JsonProperty("avatar_url")]
    public string AvatarUrl { get; set; } = default!;

    [JsonProperty("gravatar_id")]
    public string? GravatarId { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; } = default!;

    [JsonProperty("html_url")]
    public string HtmlUrl { get; set; } = default!;

    [JsonProperty("followers_url")]
    public string FollowersUrl { get; set; } = default!;

    [JsonProperty("following_url")]
    public string FollowingUrl { get; set; } = default!;

    [JsonProperty("gists_url")]
    public string GistsUrl { get; set; } = default!;

    [JsonProperty("starred_url")]
    public string StarredUrl { get; set; } = default!;

    [JsonProperty("subscriptions_url")]
    public string SubscriptionsUrl { get; set; } = default!;

    [JsonProperty("organizations_url")]
    public string OrganizationsUrl { get; set; } = default!;

    [JsonProperty("repos_url")]
    public string ReposUrl { get; set; } = default!;

    [JsonProperty("events_url")]
    public string EventsUrl { get; set; } = default!;

    [JsonProperty("received_events_url")]
    public string ReceivedEventsUrl { get; set; } = default!;

    [JsonProperty("type")]
    public string Type { get; set; } = default!;

    [JsonProperty("site_admin")]
    public bool SiteAdmin { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("company")]
    public string? Company { get; set; }

    [JsonProperty("blog")]
    public string? Blog { get; set; }

    [JsonProperty("location")]
    public string? Location { get; set; }

    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("notification_email")]
    public string? NotificationEmail { get; set; }

    [JsonProperty("hireable")]
    public bool? Hireable { get; set; }

    [JsonProperty("bio")]
    public string? Bio { get; set; }

    [JsonProperty("twitter_username")]
    public string? TwitterUsername { get; set; }

    [JsonProperty("public_repos")]
    public int PublicRepos { get; set; }

    [JsonProperty("public_gists")]
    public int PublicGists { get; set; }

    [JsonProperty("followers")]
    public int Followers { get; set; }

    [JsonProperty("following")]
    public int Following { get; set; }

    [JsonProperty("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    // Ниже поля, которые есть только у Private User в схеме
    [JsonProperty("private_gists")]
    public int? PrivateGists { get; set; }

    [JsonProperty("total_private_repos")]
    public int? TotalPrivateRepos { get; set; }

    [JsonProperty("owned_private_repos")]
    public int? OwnedPrivateRepos { get; set; }

    [JsonProperty("disk_usage")]
    public int? DiskUsage { get; set; }

    [JsonProperty("collaborators")]
    public int? Collaborators { get; set; }

    [JsonProperty("two_factor_authentication")]
    public bool? TwoFactorAuthentication { get; set; }

    [JsonProperty("business_plus")]
    public bool? BusinessPlus { get; set; }

    [JsonProperty("ldap_dn")]
    public string? LdapDn { get; set; }

    [JsonProperty("plan")]
    public GitHubUserPlanDto? Plan { get; set; }
}

public sealed class GitHubUserPlanDto
{
    [JsonProperty("collaborators")]
    public int Collaborators { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = default!;

    [JsonProperty("space")]
    public int Space { get; set; }

    [JsonProperty("private_repos")]
    public int PrivateRepos { get; set; }
}
