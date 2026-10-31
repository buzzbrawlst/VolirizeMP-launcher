using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VolirizeMP.Launcher.Models;
using VolirizeMP.Launcher.Services;

namespace VolirizeMP.Launcher;

public partial class MainWindow : Window
{
    private readonly ApiService _api = new();
    private string? _token;
    private UserProfile _user = new();

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _token = SessionStore.Load();
        if (!string.IsNullOrWhiteSpace(_token))
        {
            var profile = await _api.GetMeAsync(_token);
            if (profile is not null) SetUser(profile);
            else { SessionStore.Clear(); _token = null; }
        }

        await CheckBackendAsync();
        await LoadLatestNewsAsync();
    }

    private void SetUser(UserProfile user)
    {
        _user = user;
        var display = string.IsNullOrWhiteSpace(user.GlobalName) ? user.Username : user.GlobalName;
        SignedInText.Text = display;
        HomeUsername.Text = display;
        AccountUsername.Text = display;
        AccountId.Text = string.IsNullOrWhiteSpace(user.Id) ? "Account connected" : $"Discord ID: {user.Id}";
    }

    private async Task CheckBackendAsync()
    {
        var online = await _api.CheckBackendAsync();
        StatusText.Text = online ? "Backend online" : "Backend unavailable";
        StatusDot.Fill = online ? Brushes.LightGreen : Brushes.IndianRed;
    }

    private async Task LoadLatestNewsAsync()
    {
        try
        {
            var articles = await _api.GetNewsAsync(_token);
            if (articles.Count == 0) return;
            LatestNewsTitle.Text = articles[0].Title;
            LatestNewsDescription.Text = articles[0].Description;
        }
        catch
        {
            LatestNewsTitle.Text = "News not connected";
            LatestNewsDescription.Text = "Check Config.ApiBaseUrl and your backend /news endpoint.";
        }
    }

    private void ShowPage(string page)
    {
        HomeView.Visibility = page == "home" ? Visibility.Visible : Visibility.Collapsed;
        AccountView.Visibility = page == "account" ? Visibility.Visible : Visibility.Collapsed;
        PageBrowser.Visibility = page is "news" or "downloads" ? Visibility.Visible : Visibility.Collapsed;
        PageTitle.Text = page switch { "home" => "Home", "news" => "News", "downloads" => "Downloads", _ => "Account" };

        HomeButton.Background = page == "home" ? new SolidColorBrush(Color.FromRgb(42, 46, 67)) : Brushes.Transparent;
        NewsButton.Background = page == "news" ? new SolidColorBrush(Color.FromRgb(42, 46, 67)) : Brushes.Transparent;
        DownloadsButton.Background = page == "downloads" ? new SolidColorBrush(Color.FromRgb(42, 46, 67)) : Brushes.Transparent;
        AccountButton.Background = page == "account" ? new SolidColorBrush(Color.FromRgb(42, 46, 67)) : Brushes.Transparent;
    }

    private async Task NavigateHtmlAsync(string fileName)
    {
        ShowPage(fileName == "news.html" ? "news" : "downloads");
        try
        {
            await PageBrowser.EnsureCoreWebView2Async();
            var path = Path.Combine(AppContext.BaseDirectory, "WebPages", fileName);
            if (!File.Exists(path))
            {
                MessageBox.Show($"Could not find {path}", "VolirizeMP", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            PageBrowser.Source = new Uri(path);
        }
        catch (Exception ex)
        {
            MessageBox.Show("WebView2 could not start. Install the Microsoft Edge WebView2 Runtime.\n\n" + ex.Message,
                "VolirizeMP", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Home_Click(object sender, RoutedEventArgs e) => ShowPage("home");
    private async void News_Click(object sender, RoutedEventArgs e) => await NavigateHtmlAsync("news.html");
    private async void Downloads_Click(object sender, RoutedEventArgs e) => await NavigateHtmlAsync("downloads.html");
    private void Account_Click(object sender, RoutedEventArgs e) => ShowPage("account");

    private void Discord_Click(object sender, RoutedEventArgs e)
    {
        if (!Config.DiscordInviteUrl.Contains("YOUR_INVITE"))
            Process.Start(new ProcessStartInfo(Config.DiscordInviteUrl) { UseShellExecute = true });
        else
            MessageBox.Show("Set Config.DiscordInviteUrl first.", "VolirizeMP");
    }

    private void SignOut_Click(object sender, RoutedEventArgs e)
    {
        SessionStore.Clear();
        _token = null;
        SetUser(new UserProfile());
        SignedInText.Text = "Not signed in";
    }
}