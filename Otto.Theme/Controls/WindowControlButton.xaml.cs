using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;


namespace Otto.Theme.Controls
{
    /// <summary>
    /// Interaction logic for WindowControlButton.xaml
    /// </summary>
    public partial class WindowControlButton : INotifyPropertyChanged
    {
        public static readonly DependencyProperty FocusBrushProperty = DependencyProperty.Register(
            nameof(FocusBrush), typeof(Brush), typeof(WindowControlButton),
            new PropertyMetadata(null, (owner, _) => ((WindowControlButton)owner).OnPropertyChanged(nameof(FocusBrush))));

        public Brush FocusBrush
        {
            get => (Brush)GetValue(FocusBrushProperty);
            set => SetValue(FocusBrushProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(WindowControlButton), new PropertyMetadata(default(string)));

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

       
        public WindowControlButton()
        {
            InitializeComponent();

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
