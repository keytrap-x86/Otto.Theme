
using Otto.Theme.Controls;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Window = Otto.Theme.Controls.Window;

// ReSharper disable AssignNullToNotNullAttribute

namespace Otto.Theme.Themes
{
    public partial class Generic
    {
        private void GrdDragForm_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(System.Windows.Window.GetWindow(sender as Grid) is Window window))
                return;

            if (e.ChangedButton == MouseButton.Left)
                window.DragMove();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            if (sender != null && Window.GetWindow(sender as WindowControlButton) is Window window)
                window.WindowState = WindowState.Minimized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (sender != null && Window.GetWindow(sender as WindowControlButton) is Window window)
                window.Close();
        }

        private void BtnMaximize_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender == null || !(Window.GetWindow(sender as WindowControlButton) is Window window)) 
                return;

            switch (window.WindowState)
            {
                case WindowState.Maximized:
                    window.WindowState = WindowState.Normal;
                    break;
                case WindowState.Normal:
                    window.WindowState = WindowState.Maximized;
                    break;
            }
        }
    }
}
