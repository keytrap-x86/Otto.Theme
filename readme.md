# Otto.Theme

Otto is a .NET 10 desktop theme inspired by the original Discord and JetBrains Rider palette. The repository contains the Windows WPF theme and a separate Avalonia implementation, with light and dark palettes, reusable color tokens, and interactive control galleries.

| Project | Target | Purpose |
| --- | --- | --- |
| `Otto.Theme` | `net10.0-windows` | WPF library; package ID `Otto.Theme.WPF` |
| `Otto.Theme.Demo` | `net10.0-windows` | WPF control gallery with live theme switching |
| `Otto.Theme.Avalonia` | `net10.0` | Independent Avalonia library; package ID `Otto.Theme.Avalonia` |
| `Otto.Theme.Avalonia.Demo` | `net10.0` | Avalonia gallery and Windows x64 Native AOT publish profile |
| `tests/Otto.Theme.SmokeTests` | `net10.0-windows` | WPF resource, interaction, and rendering checks |

The libraries share a visual language and token names. They do not reference each other. Avalonia uses its own native control templates; WPF-specific window chrome and `GlowWindow` remain in the WPF library.

## Build and run

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0). `global.json` selects .NET 10 and permits newer stable feature bands in that release.

Build the full solution on Windows:

```powershell
dotnet restore Otto.Theme.sln
dotnet build Otto.Theme.sln -c Release --no-restore
dotnet run --project Otto.Theme.Demo -c Release --no-build
dotnet run --project Otto.Theme.Avalonia.Demo -c Release --no-build
```

The Avalonia projects can be built separately from the Windows-only WPF solution. The supplied native publish profile and CI currently target `win-x64`; other native targets require their corresponding operating system and toolchain.

Both galleries expose a light/dark switch and examples of input, selection, data, navigation, and date controls.

## WPF integration

Reference `Otto.Theme/Otto.Theme.csproj`, or consume an `Otto.Theme.WPF` package built from this repository. Import one `Theme` dictionary in `App.xaml`:

```xaml
<Application
    x:Class="YourApp.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:otto="clr-namespace:Otto.Theme.Themes;assembly=Otto.Theme"
    StartupUri="MainWindow.xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <otto:Theme Skin="Dark" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

Use `Skin="Light"` for a light initial appearance. Standard WPF controls pick up the implicit styles automatically. The `Theme` dictionary includes both styles and its selected palette; a separate `SkinDefault.xaml` merge is unnecessary.

To switch an existing application without recreating its controls:

```csharp
using System.Linq;
using System.Windows;
using Otto.Theme.Data.Enum;
using OttoTheme = Otto.Theme.Themes.Theme;

var theme = Application.Current.Resources.MergedDictionaries
    .OfType<OttoTheme>()
    .Single();

theme.Skin = SkinType.Light; // or SkinType.Dark
```

The legacy `SkinType.Default` and `SkinType.Violet` values remain compatible with the dark palette. Directly merging `/Otto.Theme;component/Themes/Theme.xaml` also provides the default styles; use the `Theme` wrapper when you need live palette switching.

Optional custom window chrome remains available:

```xaml
<otto:GlowWindow
    x:Class="YourApp.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:otto="clr-namespace:Otto.Theme.Controls;assembly=Otto.Theme"
    Title="Your application"
    Width="1100"
    Height="760"
    Style="{StaticResource GlowWindow}">
    <Grid />
</otto:GlowWindow>
```

Its code-behind must derive from `Otto.Theme.Controls.GlowWindow`. A standard `Window` also works.

WPF button variants retain the named styles `Basic`, `Success`, `Info`, `Warning`, `Error`, and their `Filled` variants, for example `Style="{StaticResource SuccessFilled}"`.

## Avalonia integration

Reference `Otto.Theme.Avalonia/Otto.Theme.Avalonia.csproj`, or consume the corresponding locally built package. The library uses Avalonia 12.1.2 and includes its Fluent base theme.

```xml
<Application
    x:Class="YourApp.App"
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:otto="using:Otto.Theme.Avalonia"
    RequestedThemeVariant="Dark">
    <Application.Styles>
        <otto:OttoTheme />
    </Application.Styles>
</Application>
```

Do not add another `FluentTheme` alongside `OttoTheme`. To change appearance:

```csharp
using Avalonia;
using Avalonia.Styling;

Application.Current!.RequestedThemeVariant = ThemeVariant.Light;
// ThemeVariant.Dark selects the dark palette.
```

The theme follows the requested variant and does not force dark mode. Use `DynamicResource` for theme-dependent resources in your own controls.

Available convenience classes include:

```xml
<Button Classes="primary" Content="Continue" />
<Button Classes="success" Content="Success" />
<Button Classes="info" Content="Information" />
<Button Classes="warning" Content="Warning" />
<Button Classes="danger" Content="Delete" />
<TextBlock Classes="muted" Text="Supporting information" />
<Border Classes="card">
    <TextBlock Text="Card content" />
</Border>
```

The `accent` button class from Fluent is also available. The status classes select appropriate foreground colors for each palette.

## Color tokens

Palettes separate colors from templates. Common semantic resources include:

| Meaning | Shared WPF/Avalonia names | Avalonia aliases |
| --- | --- | --- |
| Application background | `BackgroundColor`, `BackgroundBrush` | `OttoAppBackgroundColor`, `OttoBackgroundBrush` |
| Surface and raised surface | `DarkestBrush`, `LightBackgroundBrush` | `OttoSurfaceBrush`, `OttoRaisedBrush` |
| Main and secondary text | `PrimaryTextBrush`, `SecondaryTextBrush` | `OttoForegroundBrush`, `OttoMutedForegroundBrush` |
| Control background, text, border | `ControlBackgroundBrush`, `ControlForegroundBrush`, `ControlBorderBrush` | `OttoControlBackgroundBrush`, `OttoControlForegroundBrush`, `OttoBorderBrush` |
| Hover and keyboard focus | `ControlHoveredBackgroundBrush`, `ControlFocusedBorderBrush`, `FocusRingBrush` | `OttoHoverBrush`, `OttoFocusBorderBrush` |
| Disabled states | `ControlDisabledBackgroundBrush`, `ControlDisabledForegroundBrush` | `OttoDisabledBackgroundBrush`, `OttoDisabledForegroundBrush` |
| Accent and text on accent | `PrimaryBrush`, `OnAccentBrush` | `OttoAccentBrush`, `OttoAccentForegroundBrush` |
| Selection | `SelectionInactiveBrush` | `OttoSelectedBrush` |
| Status | `SuccessBrush`, `InfoBrush`, `WarningBrush`, `ErrorBrush` | `OttoSuccessBrush`, `OttoInfoBrush`, `OttoWarningBrush`, `OttoErrorBrush` |

The shared names describe equivalent roles; their exact palette values can differ between frameworks. Colors have corresponding `Color` keys, and brushes refer to those colors dynamically.

For WPF, use `Theme.ColorOverrides` so nested brush resources receive the customized colors. Define overrides on the theme dictionary:

```xaml
<otto:Theme Skin="Dark">
    <otto:Theme.ColorOverrides>
        <ResourceDictionary>
            <Color x:Key="PrimaryColor">#6B46C1</Color>
            <Color x:Key="PrimaryLightColor">#8966D6</Color>
            <Color x:Key="PrimaryDarkColor">#55359F</Color>
            <Color x:Key="ControlFocusedBorderColor">#6B46C1</Color>
            <Color x:Key="ControlHoveredBorderColor">#8966D6</Color>
            <Color x:Key="FocusRingColor">#8966D6</Color>
        </ResourceDictionary>
    </otto:Theme.ColorOverrides>
</otto:Theme>
```

Use `SetColor` to update an existing color token immediately:

```csharp
using System.Windows.Media;

theme.SetColor("PrimaryColor", Color.FromRgb(107, 70, 193));
```

For several changes, edit `ColorOverrides` and refresh once:

```csharp
theme.ColorOverrides["PrimaryColor"] = Color.FromRgb(107, 70, 193);
theme.ColorOverrides["ControlFocusedBorderColor"] = Color.FromRgb(107, 70, 193);
theme.Refresh();
```

These overrides persist when `Theme.Skin` changes. `ColorOverrides` accepts existing color-token names and `Color` values; replacing the dictionary applies it immediately, while editing it in place requires `Refresh()`. Use this API instead of relying on application-level `Color` resources to refresh the theme's nested brushes. The full WPF palettes live in `Themes/Basic/Colors.xaml` and `Themes/Basic/LightColors.xaml`.

For Avalonia, customize the `Otto*` color keys in application resources, using separate `Light` and `Dark` theme dictionaries when values should change with appearance:

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.ThemeDictionaries>
            <ResourceDictionary x:Key="Light">
                <Color x:Key="OttoAppBackgroundColor">#F5F3FA</Color>
            </ResourceDictionary>
            <ResourceDictionary x:Key="Dark">
                <Color x:Key="OttoAppBackgroundColor">#24212B</Color>
            </ResourceDictionary>
        </ResourceDictionary.ThemeDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

The complete Avalonia tokens and Fluent palettes are in `Otto.Theme.Avalonia/OttoTheme.axaml`. Fluent provides the standard templates and their interactive states; Otto supplies the palette and targeted style overrides.

## Control coverage

| Category | WPF | Avalonia |
| --- | --- | --- |
| Actions | Button, RepeatButton, ToggleButton | Button, RepeatButton, ToggleButton, HyperlinkButton |
| Selection | CheckBox, RadioButton, ComboBox | CheckBox, RadioButton, ComboBox, ToggleSwitch |
| Text and numbers | TextBox, PasswordBox, RichTextBox, Label | TextBox, password entry via `PasswordChar`, NumericUpDown, AutoCompleteBox, TextBlock, SelectableTextBlock |
| Ranges and feedback | Slider in both orientations, ProgressBar, ToolTip | Slider, ProgressBar, ToolTip |
| Collections | ListBox, ListView/GridView, TreeView, editable DataGrid | ListBox, TreeView, native TableView with typed editing in the gallery |
| Containers | GroupBox, GridSplitter, Expander, TabControl/TabItem | GroupBox, GridSplitter, Expander, TabControl/TabItem |
| Dates | Calendar, DatePicker | Calendar, CalendarDatePicker, DatePicker, TimePicker |
| Menus | Menu, nested MenuItem, ContextMenu, separators | Menu, MenuItem, ContextMenu, separators |
| Layout and chrome | ScrollViewer, ScrollBar, GridSplitter, ToolBar/ToolBarTray, StatusBar, Window/GlowWindow | ScrollViewer, ScrollBar, GridSplitter, Window and themed card surfaces |

Controls retain their framework interaction model, including keyboard navigation, focus, selection, popup handling, and disabled states. The smoke checks exercise representative behaviors and render each gallery page in both palettes.

The Avalonia gallery uses the core Avalonia 12 `TableView` for tabular data, with explicitly typed bindings and editing controls. It has no dependency on the legacy `Avalonia.Controls.DataGrid` package. `TableView` displays rows; editing is provided through typed templates or a separate editor rather than WPF `DataGrid` editing APIs.

## Native AOT

Native AOT publishing applies to the Avalonia application. WPF remains a normal .NET desktop application: Microsoft lists [WPF as incompatible with trimming](https://learn.microsoft.com/dotnet/core/deploying/trimming/incompatibilities), which is a prerequisite for Native AOT.

On Windows, install Visual Studio 2022 or later with **Desktop development with C++**, its default MSVC components, and the Windows SDK, as described in the [.NET Native AOT prerequisites](https://learn.microsoft.com/dotnet/core/deploying/native-aot/). Avalonia's [Native AOT guide](https://docs.avaloniaui.net/docs/deployment/native-aot) covers compiled XAML and binding requirements.

Publish the Avalonia gallery using the supplied profile:

```powershell
dotnet publish Otto.Theme.Avalonia.Demo -c Release -p:PublishProfile=NativeAot -o artifacts/avalonia/win-x64
```

Distribute the entire publish directory, including native dependencies. The library uses compiled XAML and the gallery uses compiled bindings and explicitly typed models. The native smoke command below checks that it is actually running without dynamic code support.

In Visual Studio, select **NativeAot** or the **FolderProfile** alias; both use the same settings. `PublishReadyToRun` and `PublishSingleFile` alone do **not** enable Native AOT. Both profiles explicitly enable `PublishAot` and disable those alternative publishing modes.

The profile uses [`OptimizationPreference=Size`](https://learn.microsoft.com/dotnet/core/deploying/native-aot/optimizing), trading some potential throughput for a smaller executable. It preserves globalization, stack traces, and hardware rendering. Debug symbols are still generated but stored separately in `artifacts/symbols/win-x64/`, including the native dependency PDBs. They are useful for crash analysis and are not needed to run the application. On Windows, `StripSymbols=true` alone does not exclude PDBs from the publish directory.

For reference, the complete gallery published with .NET 10.0.12 / Avalonia 12.1.2 on Windows x64 measured **32.1 MiB** uncompressed: **14.1 MiB** for the executable and **18.0 MiB** for Skia, HarfBuzz, and ANGLE. These native rendering libraries remain required by the configured renderer; the .NET trimmer does not shrink their precompiled DLLs. Sizes vary with compiler and dependency versions. A ZIP reduces download size, not installed size.

Native AOT is compiled for a particular operating system and architecture. Use a matching host/toolchain for other targets; [.NET does not support cross-OS Native AOT compilation](https://learn.microsoft.com/dotnet/core/deploying/native-aot/cross-compile).

## Verification and packages

After a Release build, run the WPF checks:

```powershell
dotnet run --project tests/Otto.Theme.SmokeTests -c Release --no-build
```

The WPF runner writes gallery screenshots to `artifacts/smoke`. To check the Avalonia native executable and collect its report and light/dark screenshots:

```powershell
$nativeDemo = (Resolve-Path "artifacts/avalonia/win-x64/Otto.Theme.Avalonia.Demo.exe").Path
$smokeOutput = Join-Path (Get-Location) "artifacts/avalonia-smoke"
$nativeSmoke = Start-Process -FilePath $nativeDemo -WindowStyle Hidden -PassThru -Wait -ArgumentList @(
    "--smoke-test", "--require-aot", "--smoke-output", "`"$smokeOutput`""
)
if ($nativeSmoke.ExitCode -ne 0) { throw "Avalonia native smoke test failed." }
```

The Avalonia smoke runner writes `report.txt` and a PNG for each gallery page and palette. For a regular .NET development run, omit `--require-aot`.

Build the version 2.0.0 packages locally:

```powershell
dotnet pack Otto.Theme/Otto.Theme.csproj -c Release -o artifacts/packages
dotnet pack Otto.Theme.Avalonia/Otto.Theme.Avalonia.csproj -c Release -o artifacts/packages
```

The repository's [Windows workflow](.github/workflows/build.yml) restores and builds the solution, runs the WPF checks, publishes and runs the Avalonia native executable, rejects PDBs in the distribution, reports its size, and uploads packages, the native publish directory, reports, and screenshots. Debug symbols are uploaded as a separate artifact. It does not publish packages to NuGet.org. Use the generated packages as a local feed or reference the source projects while working with this revision.

WPF bitmap capture and native menu opening require an available Windows graphics/input session. The smoke runner probes the renderer and compares unavailable menu capture with stock WPF; it reports explicit skips in remote or non-interactive sessions while continuing resource, layout, palette, binding, and data checks.
