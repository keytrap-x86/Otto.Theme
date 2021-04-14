using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Otto.Theme.Helpers;

namespace Otto.Theme.Controls
{
    /// <summary>
    ///     Border which allows Clipping to its border.
    ///     Useful especially when you need to clip to round corners.
    /// <see cref="https://wpfspark.wordpress.com/2011/06/08/clipborder-a-wpf-border-that-clips/"/>
    /// </summary>
    public class ClipBorder : Border
    {
        private Geometry clipGeometry;
        private object oldClip;

        public override UIElement Child
        {
            get => base.Child;
            set
            {
                if (Child != value)
                {
                    Child?.SetValue(ClipProperty, oldClip);

                    oldClip = value?.ReadLocalValue(ClipProperty);

                    base.Child = value;
                }
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            OnApplyChildClip();
            base.OnRender(dc);
        }

        protected virtual void OnApplyChildClip()
        {
            var child = Child;
            if (child != null)
            {
                // Get the geometry of a rounded rectangle border based on the BorderThickness and CornerRadius
                clipGeometry =
                    GeometryHelper.GetRoundRectangle(new Rect(Child.RenderSize), BorderThickness, CornerRadius);
                child.Clip = clipGeometry;
            }
        }
    }
}