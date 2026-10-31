using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VolirizeMP.Launcher.Services;

namespace VolirizeMP.Launcher.Controls;

public partial class BackgroundLayer : UserControl
{
    private readonly DispatcherTimer _gifTimer = new() { Interval = TimeSpan.FromMilliseconds(90) };
    private BitmapFrame[] _gifFrames = [];
    private int _gifIndex;

    public BackgroundLayer()
    {
        InitializeComponent();
        _gifTimer.Tick += (_, _) =>
        {
            if (_gifFrames.Length == 0) return;
            _gifIndex = (_gifIndex + 1) % _gifFrames.Length;
            GifImage.Source = _gifFrames[_gifIndex];
        };
    }

    public void SetPage(string page)
    {
        Tag = page;
        LoadMedia(BackgroundSettingsService.ResolveForPage(page));
    }

    public void Refresh()
    {
        var page = Tag as string ?? "home";
        LoadMedia(BackgroundSettingsService.ResolveForPage(page));
    }

    private void LoadMedia(string path)
    {
        Tag = Tag ?? "home";
        _gifTimer.Stop();
        Video.Stop();
        Video.Visibility = Visibility.Collapsed;
        StillImage.Visibility = Visibility.Collapsed;
        GifImage.Visibility = Visibility.Collapsed;
        StillImage.Source = null;
        GifImage.Source = null;
        _gifFrames = [];

        var settings = BackgroundSettingsService.Load();
        var blur = new BlurEffect { Radius = Math.Clamp(settings.Blur, 0, 30), KernelType = KernelType.Gaussian };
        StillImage.Effect = blur;
        GifImage.Effect = blur;
        Video.Effect = blur;

        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return;
        var extension = Path.GetExtension(path).ToLowerInvariant();
        try
        {
            if (extension == ".mp4" || extension == ".wmv" || extension == ".mov")
            {
                Video.Source = new Uri(path, UriKind.Absolute);
                Video.Visibility = Visibility.Visible;
                Video.Play();
            }
            else if (extension == ".gif")
            {
                var decoder = new GifBitmapDecoder(new Uri(path), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                _gifFrames = decoder.Frames.ToArray();
                if (_gifFrames.Length > 0)
                {
                    _gifIndex = 0;
                    GifImage.Source = _gifFrames[0];
                    GifImage.Visibility = Visibility.Visible;
                    if (_gifFrames.Length > 1) _gifTimer.Start();
                }
            }
            else
            {
                StillImage.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
                StillImage.Visibility = Visibility.Visible;
            }
        }
        catch { }
    }

    private void Video_MediaEnded(object sender, RoutedEventArgs e)
    {
        Video.Position = TimeSpan.Zero;
        Video.Play();
    }
}