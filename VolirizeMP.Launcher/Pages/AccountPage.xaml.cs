using System.Windows;
using System.Windows.Controls;

namespace VolirizeMP.Launcher.Pages;

public partial class AccountPage : UserControl
{
    private readonly Action _signOut;
    public AccountPage(string? token, Action signOut)
    {
        InitializeComponent();
        _signOut = signOut;
        var signedIn = !string.IsNullOrWhiteSpace(token);
        AccountName.Text = signedIn ? "Signed-in player" : "Guest";
        AccountStatus.Text = signedIn ? "A launcher session is stored on this device." : "You are not signed in.";
        SignOutButton.Visibility = signedIn ? Visibility.Visible : Visibility.Collapsed;
    }
    private void SignOut_Click(object sender, RoutedEventArgs e) => _signOut();
}