using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Otto.Theme.Avalonia.Demo;

public partial class MainWindow : Window
{
    public GalleryViewModel ViewModel { get; } = new();

    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
        DataContext = ViewModel;
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        Closed += (_, _) => ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(GalleryViewModel.IsDarkTheme) && Application.Current is { } application)
            application.RequestedThemeVariant = ViewModel.IsDarkTheme ? ThemeVariant.Dark : ThemeVariant.Light;
    }

    private void OnActionClick(object? sender, RoutedEventArgs args) => ViewModel.PerformAction();
    private void OnResetClick(object? sender, RoutedEventArgs args) => ViewModel.Reset();
    private void OnSortClick(object? sender, RoutedEventArgs args) => ViewModel.SortProjects();
    private void OnIncrementClick(object? sender, RoutedEventArgs args) => ViewModel.Quantity = Math.Min(100, (ViewModel.Quantity ?? 0) + 1);

    internal T RequiredControl<T>(string name) where T : Control =>
        this.FindControl<T>(name) ?? throw new InvalidOperationException($"Missing control: {name}");
}
