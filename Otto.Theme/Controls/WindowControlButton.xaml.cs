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
        private Brush _focusBrush;
        public Brush FocusBrush
        {
            get => _focusBrush;
            set
            {
                if (Equals(value, _focusBrush))
                    return;
                _focusBrush = value;
                OnPropertyChanged();
            }
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
