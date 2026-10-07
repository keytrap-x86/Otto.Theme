using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Otto.Theme.Avalonia.Demo;

public abstract class ObservableModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}

public sealed class ProjectRow(string name, string status, decimal progress, bool active) : ObservableModel
{
    private string _name = name;
    private string _status = status;
    private decimal _progress = progress;
    private bool _active = active;

    public string Name { get => _name; set => Set(ref _name, value); }
    public string Status { get => _status; set => Set(ref _status, value); }
    public decimal Progress { get => _progress; set => Set(ref _progress, value); }
    public bool Active { get => _active; set => Set(ref _active, value); }
}

public sealed class NavigationNode(string name, params NavigationNode[] children)
{
    public string Name { get; } = name;
    public IReadOnlyList<NavigationNode> Children { get; } = children;
}

public sealed class GalleryViewModel : ObservableModel
{
    private string _userName = "Camille";
    private string _feedback = "Les contrôles partagent la même palette et restent accessibles au clavier.";
    private string _statusMessage = "Prêt";
    private double _intensity = 65;
    private decimal? _quantity = 8;
    private bool _notificationsEnabled = true;
    private bool _isDarkTheme = true;
    private DateTimeOffset? _selectedDate = DateTimeOffset.Now.Date;
    private DateTime? _calendarDate = DateTime.Today;
    private TimeSpan? _selectedTime = new(9, 30, 0);
    private ProjectRow? _selectedProject;
    private int _actionCount;

    public GalleryViewModel() => SelectedProject = Projects[0];

    public string UserName { get => _userName; set => Set(ref _userName, value); }
    public string Feedback { get => _feedback; private set => Set(ref _feedback, value); }
    public string StatusMessage { get => _statusMessage; private set => Set(ref _statusMessage, value); }
    public double Intensity { get => _intensity; set => Set(ref _intensity, value); }
    public decimal? Quantity { get => _quantity; set => Set(ref _quantity, value); }
    public bool NotificationsEnabled { get => _notificationsEnabled; set => Set(ref _notificationsEnabled, value); }
    public bool IsDarkTheme { get => _isDarkTheme; set => Set(ref _isDarkTheme, value); }
    public DateTimeOffset? SelectedDate { get => _selectedDate; set => Set(ref _selectedDate, value); }
    public DateTime? CalendarDate { get => _calendarDate; set => Set(ref _calendarDate, value); }
    public TimeSpan? SelectedTime { get => _selectedTime; set => Set(ref _selectedTime, value); }
    public ProjectRow? SelectedProject { get => _selectedProject; set => Set(ref _selectedProject, value); }
    public int ActionCount { get => _actionCount; private set => Set(ref _actionCount, value); }

    public ObservableCollection<ProjectRow> Projects { get; } =
    [
        new("Otto · bibliothèque", "En cours", 85, true),
        new("Galerie de composants", "En cours", 70, true),
        new("Documentation", "À relire", 55, false),
        new("Application bureau", "Planifié", 25, false)
    ];

    public IReadOnlyList<NavigationNode> Navigation { get; } =
    [
        new("Otto.Theme", new("Contrôles", new("Saisie"), new("Navigation"), new("Données")),
            new("Ressources", new("Couleurs"), new("Typographie")), new("Exemples"))
    ];

    public void PerformAction()
    {
        ActionCount++;
        Feedback = $"Bonjour {UserName} ! Action exécutée {ActionCount} fois.";
        StatusMessage = "Action exécutée";
    }

    public void SortProjects()
    {
        var sorted = Projects.OrderBy(project => project.Name, StringComparer.CurrentCulture).ToArray();
        var selection = SelectedProject;
        Projects.Clear();
        foreach (var project in sorted)
            Projects.Add(project);
        SelectedProject = selection;
        StatusMessage = "Projets triés par nom";
    }

    public void Reset()
    {
        UserName = "Camille";
        Intensity = 65;
        Quantity = 8;
        NotificationsEnabled = true;
        SelectedDate = DateTimeOffset.Now.Date;
        CalendarDate = DateTime.Today;
        SelectedTime = new TimeSpan(9, 30, 0);
        ActionCount = 0;
        Feedback = "Valeurs réinitialisées.";
        StatusMessage = "Prêt";
    }
}
