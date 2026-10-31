using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using VolirizeMP.Launcher.Services;

namespace VolirizeMP.Launcher.Pages;

public partial class SettingsPage : UserControl
{
    private string? _chosenPath;
    public event Action? SettingsChanged;

    public SettingsPage()
    {
        InitializeComponent();
        var settings = BackgroundSettingsService.Load();
        BlurSlider.Value = settings.Blur;
        UpdateSelectedFile();
    }

    private string SelectedScope => (ScopeBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Every page (global)";
    private string ScopeKey => SelectedScope switch
    {
        "Home page" => "home", "News page" => "news", "Downloads page" => "downloads",
        "Account page" => "account", "Settings page" => "settings", _ => "Global"
    };

    private void ChooseMedia_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Choose a launcher background",
            Filter = "Background media|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.mp4;*.wmv;*.mov|Images|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Videos|*.mp4;*.wmv;*.mov|All files|*.*"
        };
        if (dialog.ShowDialog() == true)
        {
            _chosenPath = dialog.FileName;
            SelectedFileText.Text = System.IO.Path.GetFileName(_chosenPath);
            SaveStatus.Text = "Media selected. Save to apply it.";
        }
    }

    private void ScopeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded) return;
        _chosenPath = null;
        UpdateSelectedFile();
    }

    private void UpdateSelectedFile()
    {
        var settings = BackgroundSettingsService.Load();
        var path = ScopeKey == "Global" ? settings.GlobalMedia : settings.PageMedia.GetValueOrDefault(ScopeKey, "");
        _chosenPath = null;
        SelectedFileText.Text = !string.IsNullOrWhiteSpace(path) && System.IO.File.Exists(path)
            ? System.IO.Path.GetFileName(path) : "No media selected for this scope";
    }

    private void BlurSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (BlurValue is not null) BlurValue.Text = ((int)BlurSlider.Value).ToString();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var settings = BackgroundSettingsService.Load();
            settings.Blur = BlurSlider.Value;
            settings.Scope = ScopeKey;
            if (!string.IsNullOrWhiteSpace(_chosenPath))
            {
                var imported = BackgroundSettingsService.ImportMedia(_chosenPath, ScopeKey);
                if (ScopeKey == "Global") settings.GlobalMedia = imported;
                else settings.PageMedia[ScopeKey] = imported;
            }
            BackgroundSettingsService.Save(settings);
            SaveStatus.Text = "Background settings saved.";
            SettingsChanged?.Invoke();
            UpdateSelectedFile();
        }
        catch (Exception ex) { SaveStatus.Text = "Couldn't save settings: " + ex.Message; }
    }
}