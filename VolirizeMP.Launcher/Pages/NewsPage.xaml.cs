using System.Windows.Controls;
using VolirizeMP.Launcher.Services;

namespace VolirizeMP.Launcher.Pages;

public partial class NewsPage : UserControl
{
    private readonly ApiService _api;
    private readonly string? _token;
    public NewsPage(ApiService api, string? token)
    {
        InitializeComponent();
        _api = api; _token = token;
        Loaded += async (_, _) =>
        {
            try
            {
                var articles = await _api.GetNewsAsync(_token);
                NewsList.ItemsSource = articles;
                StatusText.Text = articles.Count == 0 ? "No news has been published yet." : $"{articles.Count} update(s)";
            }
            catch { StatusText.Text = "News is unavailable right now. Check your backend connection."; }
        };
    }
}