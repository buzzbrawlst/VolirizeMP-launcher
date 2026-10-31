namespace VolirizeMP.Launcher;

/// <summary>Change these values to connect your own API and Discord application.</summary>
public static class Config
{
    // Examples: https://api.example.com or http://203.0.113.10:3000
    public const string ApiBaseUrl = "https://YOUR-BACKEND-URL";
    public const string DiscordClientId = "YOUR_DISCORD_CLIENT_ID";
    public const string DiscordRedirectUri = "http://127.0.0.1:43821/callback/";
    public const string DiscordScopes = "identify";

    public const string LauncherName = "VolirizeMP";
    public const string LauncherVersion = "1.0.0";
    public const string DiscordInviteUrl = "https://discord.gg/YOUR_INVITE";
}