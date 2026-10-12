using LibVLCSharp.Shared;
using System.Windows.Forms;

namespace GiftCH3CH2OH;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        using var window = new Form();
        window.FormBorderStyle = FormBorderStyle.None;
        window.StartPosition = FormStartPosition.Manual;
        window.Bounds = Screen.PrimaryScreen?.Bounds
                        ?? throw new InvalidOperationException("No primary display was found.");
        window.ShowInTaskbar = false;
        window.Show();

        using var libvlc = new LibVLC(enableDebugLogs: false);
        using var videoStream = typeof(Program).Assembly.GetManifestResourceStream("GiftCH3CH2OH.video.mp4")
            ?? throw new InvalidOperationException("The embedded video resource could not be found.");
        using var mediaInput = new StreamMediaInput(videoStream);
        using var media = new Media(libvlc, mediaInput);
        using var mediaplayer = new MediaPlayer(media);
        

        var hwnd = window.Handle;
        if (hwnd == IntPtr.Zero)
            throw new InvalidOperationException("The playback window handle could not be created.");

        mediaplayer.Hwnd = hwnd;
        mediaplayer.Scale = 0;
        mediaplayer.AspectRatio = $"{window.ClientSize.Width}:{window.ClientSize.Height}";

        if (!WallpaperUtils.SetWallpaper(hwnd))
            throw new InvalidOperationException("The desktop WorkerW window could not be found.");

        mediaplayer.EndReached += (_, _) =>
        {
            Environment.Exit(0);
        };

        if (!mediaplayer.Play())
            throw new InvalidOperationException("LibVLC could not start playback.");

        Application.Run(window);
    }
}
