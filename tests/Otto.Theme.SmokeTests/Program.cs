using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Otto.Theme.Demo;

internal static class Program
{
    private static int _checks;
    private static int _screenshots;
    private static bool _rendererUnavailable;

    [STAThread]
    private static int Main(string[] args)
    {
        System.Windows.Media.RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
        var application = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        try
        {
            var xamlTheme = (Otto.Theme.Themes.Theme)System.Windows.Markup.XamlReader.Parse("""
                <otto:Theme xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                            xmlns:otto="clr-namespace:Otto.Theme.Themes;assembly=Otto.Theme" Skin="Light">
                    <otto:Theme.ColorOverrides><ResourceDictionary><Color x:Key="PrimaryColor">#9370DB</Color></ResourceDictionary></otto:Theme.ColorOverrides>
                </otto:Theme>
                """);
            Check((Color)xamlTheme["PrimaryColor"] == Colors.MediumPurple, "XAML color override is applied");
            application.Resources.MergedDictionaries.Add(new Otto.Theme.Themes.Theme());
            Type[] controlTypes = [typeof(Button), typeof(CheckBox), typeof(RadioButton), typeof(ToggleButton), typeof(RepeatButton),
                typeof(TextBox), typeof(PasswordBox), typeof(ComboBox), typeof(Slider), typeof(ProgressBar), typeof(ScrollBar),
                typeof(ListBox), typeof(ListBoxItem), typeof(ListView), typeof(ListViewItem), typeof(GridViewColumnHeader),
                typeof(TreeView), typeof(TreeViewItem), typeof(DataGrid), typeof(DataGridCell), typeof(DataGridRow),
                typeof(DataGridColumnHeader), typeof(GroupBox), typeof(GridSplitter), typeof(Expander), typeof(Label), typeof(Separator),
                typeof(RichTextBox), typeof(ToolBar), typeof(ToolBarTray), typeof(StatusBar), typeof(StatusBarItem),
                typeof(Calendar), typeof(DatePicker), typeof(TabControl), typeof(TabItem), typeof(ToolTip), typeof(ContextMenu), typeof(Menu)];
            foreach (var type in controlTypes)
                Check(application.TryFindResource(type) is Style, $"Implicit style: {type.Name}");

            var window = new MainWindow { ShowInTaskbar = false, Left = -20000, Top = -20000, WindowStartupLocation = WindowStartupLocation.Manual };
            window.Show();
            var tabs = (TabControl)window.FindName("GalleryTabs");
            var output = Path.GetFullPath(args.Length > 0 ? args[0] : "artifacts/smoke");
            Directory.CreateDirectory(output);
            var themeSwitch = (ToggleButton)window.FindName("ThemeSwitch");
            var originalSlider = (Slider)window.FindName("Intensity");
            foreach (var dark in new[] { true, false })
            {
                themeSwitch.IsChecked = dark;
                Pump(window);
                var expected = (Color)window.FindResource("BackgroundColor");
                Check(window.Background is SolidColorBrush background && background.Color == expected, "Window follows live palette");
                Check(dark ? expected.R < 80 : expected.R > 220, "Dark/light palette brightness");
                Check(ReferenceEquals(originalSlider, window.FindName("Intensity")), "Theme switch preserves controls");
            for (var i = 0; i < tabs.Items.Count; i++)
            {
                tabs.SelectedIndex = i;
                Pump(window);
                var content = (FrameworkElement)window.Content;
                Check(content.ActualWidth > 500 && content.ActualHeight > 300, $"Gallery tab {i + 1} layout");
                SaveImage(content, Path.Combine(output, $"gallery-{(dark ? "dark" : "light")}-{i + 1}.png"));
                foreach (var control in Descendants<Control>(content))
                    Check(control.Template != null || control is Separator, $"Template: {control.GetType().Name}");
                if (i == 0) CheckSelectionControls(window);
                if (i == 1) CheckDataControls(window);
                if (i == 2) CheckDatesAndMenus(window);
            }
            }
            themeSwitch.IsChecked = true;
            Pump(window);
            Check(((SolidColorBrush)window.Background).Color.R < 80, "Switch back to dark" );
            var activeTheme = (Otto.Theme.Themes.Theme)application.Resources.MergedDictionaries[0];
            activeTheme.SetColor("PrimaryColor", Colors.MediumPurple);
            Pump(window);
            Check(((SolidColorBrush)window.FindResource("PrimaryBrush")).Color == Colors.MediumPurple, "Color token override updates brush");
            themeSwitch.IsChecked = false;
            Pump(window);
            Check(((SolidColorBrush)window.FindResource("PrimaryBrush")).Color == Colors.MediumPurple, "Color token override survives mode change");
            activeTheme.ColorOverrides.Clear();
            activeTheme.Refresh();
            window.Close();
            // The library must also load through the documented URI without the Theme wrapper.
            application.Resources.MergedDictionaries.Clear();
            application.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Otto.Theme;component/Themes/Theme.xaml") });
            var standalone = new Window { Width = 400, Height = 240, ShowInTaskbar = false, Left = -20000, Content = new Button { Content = "Standalone theme" } };
            standalone.Show();
            Pump(standalone);
            Check(((Button)standalone.Content).Template != null, "Theme.xaml standalone load");
            standalone.Close();
            application.Resources.MergedDictionaries.Clear();
            var fallback = new Otto.Theme.Controls.GlowWindow { Width = 400, Height = 240, ShowInTaskbar = false, Left = -20000 };
            fallback.Show();
            Pump(fallback);
            Check(fallback.Template != null, "Generic.xaml custom window fallback");
            fallback.Close();
            Console.WriteLine($"PASS: {_checks} checks. Rendered previews: {_screenshots}. Output: {output}");
            application.Shutdown();
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            application.Shutdown(1);
            return 1;
        }
    }

    private static void CheckSelectionControls(Window window)
    {
        var slider = (Slider)window.FindName("Intensity");
        slider.Value = 40;
        var before = slider.Value;
        Slider.IncreaseSmall.Execute(null, slider);
        Check(slider.Value > before, "Slider keyboard command changes value");
        var radios = Descendants<RadioButton>(window).ToArray();
        radios[0].IsChecked = true;
        radios[1].IsChecked = true;
        Check(radios[0].IsChecked == false, "RadioButton grouping");
        var combo = Descendants<ComboBox>(window).First();
        combo.IsDropDownOpen = true;
        Pump(window);
        Check(combo.Template.FindName("PART_Popup", combo) is Popup { IsOpen: true }, "ComboBox popup");
        combo.IsDropDownOpen = false;
        var vertical = Descendants<Slider>(window).First(x => x.Orientation == Orientation.Vertical);
        Check(vertical.Template.FindName("PART_Track", vertical) is Track { Orientation: Orientation.Vertical }, "Vertical slider track");
    }

    private static void CheckDataControls(Window window)
    {
        var grid = Descendants<DataGrid>(window).Single();
        grid.Focus();
        grid.CurrentCell = new DataGridCellInfo(grid.Items[0], grid.Columns[0]);
        Check(grid.BeginEdit(), "DataGrid begins editing");
        Pump(window);
        Check(Descendants<TextBox>(grid).Any(), "DataGrid text editor exists");
        grid.CancelEdit();
        var list = Descendants<ListView>(window).Single();
        Check(Descendants<GridViewRowPresenter>(list).Any(), "ListView renders GridView columns");
        Check(Descendants<GridViewColumnHeader>(list).Any(), "GridView headers exist");
        var root = Descendants<TreeView>(window).Single().Items[0] as TreeViewItem;
        Check(root is { IsExpanded: true } && Descendants<TreeViewItem>(root).Count() >= 3, "TreeView expanded children");
    }

    private static void CheckDatesAndMenus(Window window)
    {
        var datePicker = Descendants<DatePicker>(window).First();
        datePicker.IsDropDownOpen = true;
        Pump(window);
        Check(datePicker.Template.FindName("PART_Popup", datePicker) is Popup { IsOpen: true }, "DatePicker popup opens");
        datePicker.IsDropDownOpen = false;
        var calendar = Descendants<Calendar>(window).First();
        Check(Descendants<CalendarDayButton>(calendar).Count() >= 28, "Calendar renders days");
        calendar.DisplayMode = CalendarMode.Year;
        Pump(window);
        Check(Descendants<CalendarButton>(calendar).Any(), "Calendar renders month selection");
        calendar.DisplayMode = CalendarMode.Decade;
        Pump(window);
        calendar.DisplayMode = CalendarMode.Month;
        var menu = Descendants<Menu>(window).First();
        var item = (MenuItem)menu.Items[0];
        var popup = item.Template.FindName("PART_Popup", item) as Popup;
        Check(popup is not null, "Menu submenu part exists");
        item.IsSubmenuOpen = true;
        Pump(window);
        if (!item.IsSubmenuOpen)
        {
            // Native menus require OS mouse capture, which may be denied to an
            // off-screen test window. Verify that the stock WPF menu fails too.
            var stockItem = new MenuItem { Header = "Native menu", Style = null };
            stockItem.Items.Add(new MenuItem { Header = "Child", Style = null });
            var stockMenu = new Menu { Style = null };
            stockMenu.Items.Add(stockItem);
            var stockWindow = new Window { Width = 240, Height = 160, Left = -20000, Top = -20000, ShowInTaskbar = false, Content = stockMenu };
            stockWindow.Show();
            Pump(stockWindow);
            stockItem.IsSubmenuOpen = true;
            Pump(stockWindow);
            Check(!stockItem.IsSubmenuOpen, "Menu capture unavailable for stock WPF too");
            stockWindow.Close();
            Console.WriteLine("SKIP: opening native menus requires mouse capture unavailable in off-screen windows; stock WPF behaves identically.");
            return;
        }
        Check(popup is { IsOpen: true }, "Menu submenu opens");
        var nested = (MenuItem)item.Items[1];
        nested.IsSubmenuOpen = true;
        Pump(window);
        Check(nested.Template.FindName("PART_Popup", nested) is Popup { IsOpen: true }, "Nested submenu opens");
        nested.IsSubmenuOpen = false;
        item.IsSubmenuOpen = false;
    }

    private static void Pump(Window window)
    {
        window.UpdateLayout();
        window.Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
        window.UpdateLayout();
    }

    private static IEnumerable<T> Descendants<T>(DependencyObject root) where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            if (child is T found) yield return found;
            foreach (var descendant in Descendants<T>(child)) yield return descendant;
        }
    }

    private static void SaveImage(FrameworkElement content, string path)
    {
        if (_rendererUnavailable) return;
        // WPF can return an all-transparent bitmap when Windows has no active
        // graphics session (for example after an RDP disconnect). Distinguish
        // that framework failure from a blank control/template rendering.
        var probe = new RenderTargetBitmap(16, 16, 96, 96, PixelFormats.Pbgra32);
        var probeVisual = new DrawingVisual();
        using (var context = probeVisual.RenderOpen())
            context.DrawRectangle(Brushes.Red, null, new Rect(0, 0, 16, 16));
        probe.Render(probeVisual);
        var probePixels = new int[16 * 16];
        probe.CopyPixels(probePixels, 16 * 4, 0);
        if (probePixels.All(pixel => pixel == 0))
        {
            _rendererUnavailable = true;
            Console.WriteLine("SKIP: WPF bitmap rendering is unavailable in this Windows graphics session (a plain red rectangle also renders transparent). Layout and interaction checks continue.");
            return;
        }
        var width = (int)Math.Ceiling(content.ActualWidth + content.Margin.Left + content.Margin.Right);
        var height = (int)Math.Ceiling(content.ActualHeight + content.Margin.Top + content.Margin.Bottom);
        var image = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        var drawing = new DrawingVisual();
        using (var context = drawing.RenderOpen())
            context.DrawRectangle((Brush)content.FindResource("BackgroundBrush"), null, new Rect(0, 0, width, height));
        image.Render(drawing);
        image.Render(content);
        var pixels = new int[width * height];
        image.CopyPixels(pixels, width * 4, 0);
        Check(pixels.Distinct().Take(20).Count() >= 20, "Screenshot contains rendered control content");
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(image));
        using var file = File.Create(path);
        encoder.Save(file);
        _screenshots++;
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
        _checks++;
    }
}
