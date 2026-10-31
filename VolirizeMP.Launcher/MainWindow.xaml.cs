using System.Windows;
using System.Windows.Media;
using VolirizeMP.Launcher.Models;
using VolirizeMP.Launcher.Services;
using VolirizeMP.Launcher.Pages;

namespace VolirizeMP.Launcher;

public partial class MainWindow : Window
{
    private readonly ApiService _api = new();
    private string? _token;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
        ShowPage("home");
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _token = SessionStore.Load();
        if (!string.IsNullOrWhiteSpace(_token))
        {
            var profile = await _api.GetMeAsync(_token);
            if (profile is not null) SetSignedIn(profile);
            else { SessionStore.Clear(); _token = null; }
        }
        var online = await _api.CheckBackendAsync();
        StatusText.Text = online ? "Backend online" : "Backend unavailable";
        StatusDot.Fill = online ? Brushes.LightGreen : Brushes.IndianRed;
    }

    private void SetSignedIn(UserProfile user)
    {
        SignedInText.Text = string.IsNullOrWhiteSpace(user.GlobalName) ? user.Username : user.GlobalName;
    }

    private void ShowPage(string page)
    {
        PageTitle.Text = page switch { "home" => "Home", "news" => "News", "downloads" => "Downloads", _ => "Account" };
        if (page == "home")
        {
            var home = new HomePage();
            home.NavigationRequested += (_, _) => ShowPage("downloads");
            PageHost.Content = home;
        }
        else if (page == "news") PageHost.Content = new NewsPage(_api, _token);
        else if (page == "downloads") PageHost.Content = new DownloadsPage();
        else PageHost.Content = new AccountPage(_token, SignOut);

        var active = new SolidColorBrush(Color.FromRgb(39, 45, 67));
        HomeButton.Background = page == "home" ? active : Brushes.Transparent;
        NewsButton.Background = page == "news" ? active : Brushes.Transparent;
        DownloadsButton.Background = page == "downloads" ? active : Brushes.Transparent;
        AccountButton.Background = page == "account" ? active : Brushes.Transparent;
    }

    private void SignOut()
    {
        SessionStore.Clear();
        _token = null;
        SignedInText.Text = "Not signed in";
        ShowPage("account");
    }

    private void Home_Click(object sender, RoutedEventArgs e) => ShowPage("home");
    private void News_Click(object sender, RoutedEventArgs e) => ShowPage("news");
    private void Downloads_Click(object sender, RoutedEventArgs e) => ShowPage("downloads");
    private void Account_Click(object sender, RoutedEventArgs e) => ShowPage("account");
}