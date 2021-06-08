## Otto.Theme - Simple WPF Dark Theme

Just a simple WPF dark theme inspired by Discord and little bit of JetBrain's Rider


### Nuget

[Get it from Nuget](https://www.nuget.org/packages/Otto.Theme.WPF)

```Powershell
Install-Package Otto.Theme.WPF
```

## Preview

https://user-images.githubusercontent.com/17864005/116119676-fa674100-a6be-11eb-8452-4890f07b949e.MP4


### Usage

- *App.xaml* should look like this :

```xaml
<Application
    x:Class="Otto.Theme.Demo.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="clr-namespace:Otto.Theme.Demo"
    StartupUri="MainWindow.xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="pack://application:,,,/Otto.Theme;component/Themes/SkinDefault.xaml" />
                <ResourceDictionary Source="pack://application:,,,/Otto.Theme;component/Themes/Theme.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

- *MainWindow.xaml* :

```xaml
<kt:GlowWindow
    x:Class="Otto.Theme.Demo.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    xmlns:kt="clr-namespace:Otto.Theme.Controls;assembly=Otto.Theme"
    Title="Keytrap's Dark Theme for WPF"
    Width="970"
    Height="562"
    Style="{StaticResource GlowWindow}"
    mc:Ignorable="d" >
    <Grid>

    </Grid>
</kt:GlowWindow>
```
