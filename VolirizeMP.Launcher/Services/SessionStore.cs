using System.Security.Cryptography;
using System.Text;

namespace VolirizeMP.Launcher.Services;

/// <summary>Stores the API session token encrypted for the current Windows user.</summary>
public static class SessionStore
{
    private static readonly string Folder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VolirizeMP");
    private static readonly string FilePath = Path.Combine(Folder, "session.dat");

    public static void Save(string token)
    {
        Directory.CreateDirectory(Folder);
        var bytes = Encoding.UTF8.GetBytes(token);
        var encrypted = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
        File.WriteAllBytes(FilePath, encrypted);
    }

    public static string? Load()
    {
        try
        {
            if (!File.Exists(FilePath)) return null;
            var encrypted = File.ReadAllBytes(FilePath);
            return Encoding.UTF8.GetString(
                ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser));
        }
        catch { return null; }
    }

    public static void Clear()
    {
        try { if (File.Exists(FilePath)) File.Delete(FilePath); }
        catch { }
    }
}