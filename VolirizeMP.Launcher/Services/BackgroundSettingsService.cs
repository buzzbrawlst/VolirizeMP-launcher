using System.IO;
using System.Text.Json;

namespace VolirizeMP.Launcher.Services;

public sealed class BackgroundSettings
{
    public string Scope { get; set; } = "Global";
    public string GlobalMedia { get; set; } = "";
    public Dictionary<string, string> PageMedia { get; set; } = new();
    public double Blur { get; set; } = 8;
}

public static class BackgroundSettingsService
{
    private static readonly string Folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VolirizeMP");
    private static readonly string SettingsPath = Path.Combine(Folder, "backgrounds.json");
    private static readonly string MediaFolder = Path.Combine(Folder, "Backgrounds");

    public static BackgroundSettings Load()
    {
        try
        {
            if (!File.Exists(SettingsPath)) return new BackgroundSettings();
            return JsonSerializer.Deserialize<BackgroundSettings>(File.ReadAllText(SettingsPath)) ?? new BackgroundSettings();
        }
        catch { return new BackgroundSettings(); }
    }

    public static string ImportMedia(string sourcePath, string scope)
    {
        Directory.CreateDirectory(MediaFolder);
        var extension = Path.GetExtension(sourcePath).ToLowerInvariant();
        var safeName = scope.Equals("Global", StringComparison.OrdinalIgnoreCase) ? "global" : scope.ToLowerInvariant();
        var destination = Path.Combine(MediaFolder, safeName + extension);
        File.Copy(sourcePath, destination, true);
        return destination;
    }

    public static void Save(BackgroundSettings settings)
    {
        Directory.CreateDirectory(Folder);
        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
    }

    public static string ResolveForPage(string page)
    {
        var settings = Load();
        if (settings.PageMedia.TryGetValue(page, out var specific) && File.Exists(specific)) return specific;
        return File.Exists(settings.GlobalMedia) ? settings.GlobalMedia : "";
    }
}