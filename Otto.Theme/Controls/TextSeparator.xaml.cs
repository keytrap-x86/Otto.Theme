using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace Otto.Theme.Controls
{
    /// <summary>
    /// Logique d'interaction pour TextSeparator.xaml
    /// </summary>
    public partial class TextSeparator : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private object _header;
        public object Header
        {
            get => _header;
            set
            {
                _header = value;
                OnPropertyChanged();
            }
        }

        public TextSeparator()
        {
            InitializeComponent();
        }
    }
}
