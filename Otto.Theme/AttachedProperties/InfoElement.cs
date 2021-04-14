using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Otto.Theme
{
    public class InfoElement
    {
        #region ProgressBar
        public static readonly DependencyProperty ShowProgressTextProperty =
            DependencyProperty.RegisterAttached(
                "ShowProgressText",
                typeof(bool),
                typeof(InfoElement),
                new FrameworkPropertyMetadata(false));

        public static bool GetShowProgressText(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowProgressTextProperty);
        }

        public static void SetShowProgressText(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowProgressTextProperty, value);
        }

        #endregion
    }
}