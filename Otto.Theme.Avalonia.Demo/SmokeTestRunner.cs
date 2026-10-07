using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Otto.Theme.Avalonia.Demo;

internal sealed record SmokeTestOptions(string OutputDirectory, bool RequireNativeAot)
{
    public static SmokeTestOptions? Parse(string[] args)
    {
        if (!args.Contains("--smoke-test", StringComparer.Ordinal))
            return null;

        var output = Path.Combine(AppContext.BaseDirectory, "smoke-output");
        var outputIndex = Array.IndexOf(args, "--smoke-output");
        if (outputIndex >= 0)
        {
            if (outputIndex + 1 >= args.Length)
                throw new ArgumentException("--smoke-output requires a directory path.");
            output = Path.GetFullPath(args[outputIndex + 1]);
        }

        return new SmokeTestOptions(output, args.Contains("--require-aot", StringComparer.Ordinal));
    }
}

/// <summary>Runs against the real desktop renderer, including in the published native executable.</summary>
internal static class SmokeTestRunner
{
    public static async Task<int> RunAsync(MainWindow window, SmokeTestOptions options)
    {
        var report = new StringBuilder()
            .AppendLine("Otto.Theme.Avalonia — desktop smoke test")
            .AppendLine($"UTC: {DateTimeOffset.UtcNow:O}")
            .AppendLine($"Runtime: {RuntimeInformation.FrameworkDescription}")
            .AppendLine($"Native AOT: {!RuntimeFeature.IsDynamicCodeSupported}");
        var exitCode = 0;

        try
        {
            Directory.CreateDirectory(options.OutputDirectory);
            if (options.RequireNativeAot)
                Require(!RuntimeFeature.IsDynamicCodeSupported, "The process must be a native AOT executable.");

            await SettleAsync();
            var tabs = window.RequiredControl<TabControl>("GalleryTabs");
            Require(tabs.ItemCount == 3, "Three gallery tabs are available.");
            Require(window.Bounds.Width >= 900 && window.Bounds.Height >= 600, "The window has a renderable desktop layout.");

            tabs.SelectedIndex = 0;
            await SettleAsync();
            var name = window.RequiredControl<TextBox>("NameInput");
            name.Text = "Test Otto";
            window.RequiredControl<Button>("ActionButton").RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            window.RequiredControl<Slider>("IntensitySlider").Value = 40;
            window.RequiredControl<NumericUpDown>("QuantityInput").Value = 12;
            window.RequiredControl<ToggleSwitch>("NotificationsToggle").IsChecked = false;
            await SettleAsync();
            Require(window.ViewModel.UserName == "Test Otto", "Compiled TextBox binding updates the view model.");
            Require(window.ViewModel.ActionCount == 1 && window.ViewModel.Feedback.Contains("Test Otto", StringComparison.Ordinal),
                "The action event updates the feedback.");
            Require(window.ViewModel.Intensity == 40 && window.RequiredControl<ProgressBar>("IntensityProgress").Value == 40,
                "Slider and progress bar share the same value.");
            Require(window.ViewModel.Quantity == 12 && !window.ViewModel.NotificationsEnabled,
                "NumericUpDown and ToggleSwitch bindings update the view model.");
            report.AppendLine("PASS: text, action, slider, progress, quantity and toggle bindings.");

            tabs.SelectedIndex = 1;
            await SettleAsync();
            var grid = window.RequiredControl<TableView>("ProjectsGrid");
            Require(grid.Columns.Count == 4, "The TableView has four explicit columns.");
            Require(window.RequiredControl<TreeView>("ProjectTree").ItemCount == 1, "The TreeView has its root node.");
            Require(window.RequiredControl<ListBox>("ProjectList").ItemCount == 4, "The project list has four records.");
            var edited = window.ViewModel.Projects[0];
            var originalName = edited.Name;
            window.ViewModel.SelectedProject = edited;
            await SettleAsync();
            window.RequiredControl<TextBox>("ProjectNameInput").Text = "Projet modifié";
            await SettleAsync();
            Require(edited.Name == "Projet modifié", "The typed project editor updates the selected row.");
            Require(grid.GetVisualDescendants().OfType<TextBlock>().Any(text => text.Text == "Projet modifié"),
                "The TableView compiled cell template observes property changes.");
            edited.Name = originalName;
            window.RequiredControl<Button>("SortProjectsButton").RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Require(window.ViewModel.Projects[0].Name == "Application bureau", "Typed project sorting works.");
            report.AppendLine("PASS: list/tree population, TableView cells, typed project editor and sorting.");

            tabs.SelectedIndex = 2;
            await SettleAsync();
            var date = new DateTimeOffset(2026, 10, 7, 0, 0, 0, TimeSpan.Zero);
            window.RequiredControl<DatePicker>("SampleDatePicker").SelectedDate = date;
            window.RequiredControl<TimePicker>("SampleTimePicker").SelectedTime = new TimeSpan(14, 45, 0);
            var calendar = window.RequiredControl<Calendar>("SampleCalendar");
            calendar.DisplayDate = date.Date;
            calendar.SelectedDate = date.Date;
            var expander = window.RequiredControl<Expander>("DetailsExpander");
            expander.IsExpanded = false;
            Require(!expander.IsExpanded, "The Expander collapses.");
            expander.IsExpanded = true;
            await SettleAsync();
            Require(window.ViewModel.SelectedDate == date && window.ViewModel.CalendarDate == date.Date,
                "DatePicker and Calendar bindings update the view model.");
            Require(window.ViewModel.SelectedTime == new TimeSpan(14, 45, 0), "TimePicker binding updates the view model.");
            report.AppendLine("PASS: Calendar, DatePicker, TimePicker and Expander.");

            window.ViewModel.Reset();
            tabs.SelectedIndex = 0;
            await SettleAsync();
            await VerifyAccentOverrideAsync(window, report);
            Color? darkBackground = null;
            foreach (var dark in new[] { true, false })
            {
                window.ViewModel.IsDarkTheme = dark;
                await SettleAsync();
                var expected = dark ? ThemeVariant.Dark : ThemeVariant.Light;
                Require(window.ActualThemeVariant == expected, $"The window resolves the {expected} theme.");
                var background = ReadColor(window.Background);
                var luminance = background.R + background.G + background.B;
                Require(dark ? luminance < 300 : luminance > 600, $"The {expected} background has the expected brightness.");
                if (dark)
                    darkBackground = background;
                else
                    Require(background != darkBackground, "Light and dark resolve to different background tokens.");

                for (var index = 0; index < 3; index++)
                {
                    tabs.SelectedIndex = index;
                    await SettleAsync();
                    var surface = window.GetVisualDescendants().OfType<Border>().First(border => border.Classes.Contains("card"));
                    var surfaceColor = ReadColor(surface.Background);
                    Require(dark ? surfaceColor.R + surfaceColor.G + surfaceColor.B < 360 : surfaceColor.R + surfaceColor.G + surfaceColor.B > 600,
                        $"Card surfaces follow the {expected} theme on tab {index + 1}.");
                    var fileName = $"{(dark ? "dark" : "light")}-{index + 1:00}.png";
                    Capture(window, Path.Combine(options.OutputDirectory, fileName));
                    report.AppendLine($"PASS: {expected} tab {index + 1}, background {background}, surface {surfaceColor}; {fileName}");
                }
            }

            report.AppendLine("PASS: all checks completed; six screenshots saved.");
        }
        catch (Exception exception)
        {
            exitCode = 1;
            report.AppendLine("FAIL").AppendLine(exception.ToString());
        }

        try
        {
            Directory.CreateDirectory(options.OutputDirectory);
            await File.WriteAllTextAsync(Path.Combine(options.OutputDirectory, "report.txt"), report.ToString());
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            exitCode = 1;
        }

        return exitCode;
    }

    private static async Task SettleAsync()
    {
        await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);
        await Task.Delay(100);
    }

    private static async Task VerifyAccentOverrideAsync(MainWindow window, StringBuilder report)
    {
        const string key = "OttoAccentColor";
        var application = Application.Current ?? throw new InvalidOperationException("The application is not initialized.");
        var resources = application.Resources;
        var hadOverride = resources.ContainsKey(key);
        var previousOverride = hadOverride ? resources[key] : null;
        var originalTheme = window.ViewModel.IsDarkTheme;
        var originalAccent = ReadColor(window.FindResource("OttoAccentBrush") as IBrush);
        var customAccent = Color.FromRgb(163, 45, 140);
        var checkBox = window.RequiredControl<CheckBox>("AccentCheckBox");
        var checkSurface = checkBox.GetVisualDescendants().OfType<Border>().Single(border => border.Name == "NormalRectangle");
        var originalCheckColor = ReadColor(checkSurface.Background);

        try
        {
            resources[key] = customAccent;
            foreach (var dark in new[] { true, false, true })
            {
                window.ViewModel.IsDarkTheme = dark;
                await SettleAsync();
                var token = window.FindResource(key);
                var accentBrush = ReadColor(window.FindResource("OttoAccentBrush") as IBrush);
                var checkColor = ReadColor(checkSurface.Background);
                report.AppendLine($"Accent override ({(dark ? "Dark" : "Light")}): token={token}; OttoAccentBrush={accentBrush}; checked Fluent surface={checkColor}; expected={customAccent}.");
                Require(token is Color resolved && resolved == customAccent,
                    "The application OttoAccentColor override resolves after the theme switch.");
                Require(accentBrush == customAccent,
                    "OttoAccentBrush must follow the application OttoAccentColor override.");
                Require(checkColor == customAccent,
                    "The checked Fluent CheckBox must follow the application OttoAccentColor override.");
            }
        }
        finally
        {
            if (hadOverride)
                resources[key] = previousOverride;
            else
                resources.Remove(key);
            window.ViewModel.IsDarkTheme = originalTheme;
            await SettleAsync();
        }

        Require(ReadColor(window.FindResource("OttoAccentBrush") as IBrush) == originalAccent,
            "Removing the application override restores the original OttoAccentBrush.");
        Require(ReadColor(checkSurface.Background) == originalCheckColor,
            "Removing the application override restores the checked Fluent surface.");
        report.AppendLine("PASS: application accent override updates Otto and Fluent in both themes and restores cleanly.");
    }

    private static Color ReadColor(IBrush? brush) => brush is ISolidColorBrush solid
        ? solid.Color
        : throw new InvalidOperationException("Expected a solid theme brush.");

    private static void Capture(Window window, string path)
    {
        var pixels = new PixelSize((int)Math.Ceiling(window.Bounds.Width), (int)Math.Ceiling(window.Bounds.Height));
        using var bitmap = new RenderTargetBitmap(pixels, new Vector(96, 96));
        bitmap.Render(window);
        bitmap.Save(path, PngBitmapEncoderOptions.Default);
        Require(new FileInfo(path).Length > 1_000, "The screenshot contains rendered content.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
