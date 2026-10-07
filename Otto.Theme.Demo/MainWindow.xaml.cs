using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Otto.Theme.Demo;

public partial class MainWindow : INotifyPropertyChanged
{
    public ObservableCollection<DemoProject> Projects { get; } = new()
    {
        new() { Name = "Migration .NET 10", Status = "Terminé", Progress = 100, Active = true },
        new() { Name = "Galerie des contrôles", Status = "En cours", Progress = 85, Active = true },
        new() { Name = "Documentation", Status = "En revue", Progress = 70, Active = true },
        new() { Name = "Prochain thème", Status = "Planifié", Progress = 10, Active = false }
    };

    public DateTime Today { get; } = DateTime.Today;
    private DateTime? _selectedDate = DateTime.Today;
    public DateTime? SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (_selectedDate == value) return;
            _selectedDate = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDate)));
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;
    private int _actionCount;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void OnThemeModeChanged(object sender, RoutedEventArgs e)
    {
        if (sender is not System.Windows.Controls.Primitives.ToggleButton toggle) return;
        var skin = toggle.IsChecked == true ? Data.Enum.SkinType.Dark : Data.Enum.SkinType.Light;
        foreach (var dictionary in Application.Current.Resources.MergedDictionaries)
            if (dictionary is Themes.Theme theme) theme.Skin = skin;
        toggle.Content = skin == Data.Enum.SkinType.Dark ? "Mode sombre" : "Mode clair";
    }

    private void OnActionClick(object sender, RoutedEventArgs e)
    {
        ActionFeedback.Text = $"Action exécutée {++_actionCount} fois.";
    }
}

public class DemoProject : INotifyPropertyChanged
{
    private string _name;
    private string _status;
    private int _progress;
    private bool _active;
    public string Name { get => _name; set => Set(ref _name, value); }
    public string Status { get => _status; set => Set(ref _status, value); }
    public int Progress { get => _progress; set => Set(ref _progress, value); }
    public bool Active { get => _active; set => Set(ref _active, value); }
    public event PropertyChangedEventHandler PropertyChanged;

    private void Set<T>(ref T field, T value, [CallerMemberName] string name = null)
    {
        if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
