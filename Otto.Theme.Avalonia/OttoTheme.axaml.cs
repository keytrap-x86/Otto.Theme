using global::Avalonia.Markup.Xaml;
using global::Avalonia.Styling;

namespace Otto.Theme.Avalonia;

/// <summary>
/// Adds the Otto palette and control styles, including the native Fluent templates.
/// The application's RequestedThemeVariant selects the light or dark palette.
/// </summary>
public sealed partial class OttoTheme : Styles
{
    public OttoTheme()
    {
        // Avalonia's build task replaces this call with the compiled XAML initializer.
        AvaloniaXamlLoader.Load(this);
    }
}
