using System.Windows;
using System.Windows.Media;

namespace Otto.Theme.Tools.Extension
{
    // ReSharper disable once InconsistentNaming
    public static class UIElementExtension
    {
        public static void Show(this UIElement element) => element.Visibility = Visibility.Visible;

        public static void Show(this UIElement element, bool show) => element.Visibility = show ? Visibility.Visible : Visibility.Collapsed;

        public static void Hide(this UIElement element) => element.Visibility = Visibility.Hidden;

        public static void Collapse(this UIElement element) => element.Visibility = Visibility.Collapsed;

        public static TParent FindParent<TParent>(this DependencyObject child) where TParent : DependencyObject
        {
            DependencyObject current = child;
            while (current != null && !(current is TParent))
            {
                current = VisualTreeHelper.GetParent(current);
            }
            return current as TParent;
        }

        public static void SetParentValue<TParent>(this DependencyObject child, DependencyProperty property, object value) where TParent : DependencyObject
        {
            TParent parent = child.FindParent<TParent>();
            if (parent != null)
            {
                parent.SetValue(property, value);
            }
        }
    }
}