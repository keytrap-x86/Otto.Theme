using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Otto.Theme.Avalonia.Demo;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = new MainWindow();
            if (Program.SmokeTest is { } options)
            {
                window.WindowStartupLocation = WindowStartupLocation.Manual;
                window.Position = new PixelPoint(-32000, -32000);
                window.ShowInTaskbar = false;
                window.ShowActivated = false;
                window.Opened += async (_, _) =>
                {
                    var exitCode = await SmokeTestRunner.RunAsync(window, options);
                    desktop.Shutdown(exitCode);
                };
            }

            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
