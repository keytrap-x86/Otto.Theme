using Avalonia;

namespace Otto.Theme.Avalonia.Demo;

internal static class Program
{
    internal static SmokeTestOptions? SmokeTest { get; private set; }

    [STAThread]
    public static int Main(string[] args)
    {
        try
        {
            SmokeTest = SmokeTestOptions.Parse(args);
            return BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception exception)
        {
            if (SmokeTest is null)
                throw;

            Directory.CreateDirectory(SmokeTest.OutputDirectory);
            File.WriteAllText(Path.Combine(SmokeTest.OutputDirectory, "report.txt"),
                $"FAIL — application startup{Environment.NewLine}{exception}");
            return 1;
        }
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace();
}
