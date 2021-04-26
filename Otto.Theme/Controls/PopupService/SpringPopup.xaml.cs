using System.Windows;
using System.Windows.Media;

namespace Otto.Theme.Controls
{
    /// <summary>
    /// Logique d'interaction pour UserControl1.xaml
    /// </summary>
    public partial class Pop
    {
        #region BackgroundBrush

        public static readonly DependencyProperty PopupBackgroundBrushProperty =
           DependencyProperty.Register("BackgroundBrush", typeof(Brush), typeof(DropZone), new PropertyMetadata(new BrushConverter().ConvertFromString("#18191C")));

        public Brush BackgroundBrush
        {
            get { return (Brush)GetValue(PopupBackgroundBrushProperty); }
            set { SetValue(PopupBackgroundBrushProperty, value); }
        }

        #endregion BackgroundBrush

        #region Content

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(Pop), new PropertyMetadata(null));

        #endregion Content

        public Pop()
        {
            InitializeComponent();
        }
    }
}