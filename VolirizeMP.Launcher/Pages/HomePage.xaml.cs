using System.Windows;
using System.Windows.Controls;

namespace VolirizeMP.Launcher.Pages;

public partial class HomePage : UserControl
{
    public event EventHandler? NavigationRequested;
    public HomePage() => InitializeComponent();
    private void Downloads_Click(object sender, RoutedEventArgs e) => NavigationRequested?.Invoke(this, EventArgs.Empty);
}