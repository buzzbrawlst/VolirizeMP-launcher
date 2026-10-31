namespace VolirizeMP.Launcher.Models;

public sealed class UserProfile
{
    public string Id { get; set; } = "";
    public string Username { get; set; } = "Guest";
    public string? GlobalName { get; set; }
    public string? Avatar { get; set; }
    public bool IsAdmin { get; set; }
}

public sealed class NewsArticle
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "Untitled";
    public string Description { get; set; } = "";
    public string Date { get; set; } = "";
    public string Category { get; set; } = "Update";
    public string? Image { get; set; }
}

public sealed class BackendStatus
{
    public bool Online { get; set; }
    public string? Backend { get; set; }
    public string? Version { get; set; }
}