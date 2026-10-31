using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using VolirizeMP.Launcher.Models;

namespace VolirizeMP.Launcher.Services;

/// <summary>
/// Central place for launcher-to-backend HTTP calls.
/// Adjust endpoint paths and response models to match your backend.
/// </summary>
public sealed class ApiService
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(15) };
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private static string BaseUrl
    {
        get
        {
            var value = Config.ApiBaseUrl.TrimEnd('/');
            if (value.Contains("YOUR-BACKEND-URL", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Set Config.ApiBaseUrl before using backend features.");
            return value;
        }
    }

    public async Task<bool> CheckBackendAsync(CancellationToken ct = default)
    {
        try
        {
            using var response = await Http.GetAsync(BaseUrl + "/", ct);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<UserProfile?> GetMeAsync(string token, CancellationToken ct = default)
    {
        using var request = AuthRequest(HttpMethod.Get, BaseUrl + "/auth/me", token);
        using var response = await Http.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<UserProfile>(JsonOptions, ct);
    }

    public async Task<List<NewsArticle>> GetNewsAsync(string? token = null, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + "/news");
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await Http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<NewsArticle>>(JsonOptions, ct) ?? [];
    }

    private static HttpRequestMessage AuthRequest(HttpMethod method, string url, string token)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }
}