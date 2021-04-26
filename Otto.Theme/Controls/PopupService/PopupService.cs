using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Otto.Theme.Controls
{
    /// <summary>
    ///     Class that provides the Popup attached property
    /// </summary>
    public class PopupService : DependencyObject, IDisposable
    {
        public static Pop Pop;

        /// <summary>
        ///     Popup Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty PopupProperty = DependencyProperty.RegisterAttached(
            nameof(Controls.Pop),
            typeof(object),
            typeof(PopupService),
            new FrameworkPropertyMetadata(null, OnPopChanged));

        /// <summary>
        ///     Gets the Popup property.  This dependency property indicates the Popup for the control.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject" /> to get the property from</param>
        /// <returns>The value of the Popup property</returns>
        public static object GetPop(DependencyObject d)
        {
            return d.GetValue(PopupProperty);
        }

        /// <summary>
        ///     Sets the Popup property.  This dependency property indicates the Popup for the control.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject" /> to set the property on</param>
        /// <param name="value">value of the property</param>
        public static void SetPop(DependencyObject d, object value)
        {
            d.SetValue(PopupProperty, value);
        }

        #region PopupBackgroundBrush

        /// <summary>
        ///     PopupBackgroundBrush Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty PopBackgroundBrushProperty = DependencyProperty.RegisterAttached(
            "PopBackgroundBrush",
            typeof(Brush),
            typeof(PopupService),
            new FrameworkPropertyMetadata(new BrushConverter().ConvertFromString("#18191C")));

        /// <summary>
        ///     Gets the PopupBackgroundBrush property.  This dependency property indicates the PopupBackgroundBrush for the control.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject" /> to get the property from</param>
        /// <returns>The value of the PopupBackgroundBrush property</returns>
        public static Brush GetPopBackgroundBrush(DependencyObject d)
        {
            return (Brush)d.GetValue(PopBackgroundBrushProperty);
        }

        /// <summary>
        ///     Sets the PopupBackgroundBrush property.  This dependency property indicates the PopupBackgroundBrush for the control.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject" /> to set the property on</param>
        /// <param name="value">value of the property</param>
        public static void SetPopBackgroundBrush(DependencyObject d, Brush value)
        {
            d.SetValue(PopBackgroundBrushProperty, value);
        }

        #endregion PopupBackgroundBrush

        /// <summary>
        ///     Handles changes to the Popup property.
        /// </summary>
        /// <param name="d"><see cref="DependencyObject" /> that fired the event</param>
        /// <param name="e">A <see cref="DependencyPropertyChangedEventArgs" /> that contains the event data.</param>
        private static void OnPopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (UIElement)d;

            control.MouseEnter += (s, e) =>
           {
               var content = d.GetValue(PopupProperty);

               if (content is string)
               {
                   content = BuildDefaultTextBlock(content?.ToString());
               }

               Pop = new Pop
               {
                   BackgroundBrush = control.GetValue(PopBackgroundBrushProperty) as Brush,
                   Content = content,
                   PopupAnimation = PopupAnimation.None,
                   PlacementTarget = control
               };

               if (Pop != null)
                   Pop.IsOpen = true;
           };

            control.MouseLeave += (s, e) =>
            {
                if (Pop != null)
                    Pop.IsOpen = false;
                Pop = null;
            };
        }

        #region Helper Methods

        private static TextBlock BuildDefaultTextBlock(string text)
        {
            return new TextBlock
            {
                Text = text,
                Padding = new Thickness(12),
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = Brushes.Transparent,
            };
        }

        public static T FindParent<T>(DependencyObject obj) where T : DependencyObject
        {
            while (true)
            {
                if (obj == null) return null;
                var parent = VisualTreeHelper.GetParent(obj);

                if (parent is T dependencyObject)
                    return dependencyObject;

                obj = parent;
            }
        }

        public void Dispose()
        {
            if (Pop != null)
                Pop.IsOpen = false;
            Pop = null;
        }

        #endregion Helper Methods
    }
}