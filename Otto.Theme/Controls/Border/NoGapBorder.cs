using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Otto.Theme.Controls
{
    /// <summary>
    /// Like <see cref="Border"/>, but doesn't have gap (antialiasing artifact)
    /// between border and fill. However, border overlaps background, so it'll
    /// look different for semitransparent border with a background.
    /// </summary>
    public class NoGapBorder : Border
    {
        static NoGapBorder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NoGapBorder), new FrameworkPropertyMetadata(typeof(NoGapBorder)));
        }

        // Hide parent Background to avoid double-layered background (a problem
        // when background is semitransparent).
        public new Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public static readonly new DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(NoGapBorder), new PropertyMetadata(null));

        // The root of the antialiasing artifact (gap) fix.
        protected override void OnRender(DrawingContext dc)
        {
            var geometry = GenerateRoundedRectangle(0, 0, ActualWidth, ActualHeight, CornerRadius, BorderThickness);
            dc.DrawGeometry(Background, null, geometry);
            base.OnRender(dc);
        }

        /// <summary>
        /// Generates a rounded rectangle <see cref="StreamGeometry"/>.
        /// </summary>
        private static StreamGeometry GenerateRoundedRectangle(
            double x, double y, double width, double height, CornerRadius radius, Thickness border)
        {
            var rectangle = new Rect(x, y, width, height);
            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
                PopulateContext(context, rectangle, radius, border);
            return geometry;
        }

        private static void PopulateContext(
            StreamGeometryContext context, Rect rectangle, CornerRadius radius, Thickness border)
        {
            var offsetLeft = border.Left / 2.0;
            var offsetTop = border.Top / 2.0;
            var offsetRight = border.Right / 2.0;
            var offsetBottom = border.Bottom / 2.0;

            double marginLeftTop;
            double marginTopLeft;
            double marginTopRight;
            double marginRightTop;
            double marginRightBottom;
            double marginBottomRight;
            double marginBottomLeft;
            double marginLeftBottom;

            if (IsZero(radius.TopLeft)) // Handle corners
                marginLeftTop = marginTopLeft = 0.0;
            else
            {
                marginLeftTop = radius.TopLeft + offsetLeft;
                marginTopLeft = radius.TopLeft + offsetTop;
            }

            if (IsZero(radius.TopRight))
                marginTopRight = marginRightTop = 0.0;
            else
            {
                marginTopRight = radius.TopRight + offsetTop;
                marginRightTop = radius.TopRight + offsetRight;
            }

            if (IsZero(radius.BottomRight))
                marginRightBottom = marginBottomRight = 0.0;
            else
            {
                marginRightBottom = radius.BottomRight + offsetRight;
                marginBottomRight = radius.BottomRight + offsetBottom;
            }
            if (IsZero(radius.BottomLeft))
                marginBottomLeft = marginLeftBottom = 0.0;
            else
            {
                marginBottomLeft = radius.BottomLeft + offsetBottom;
                marginLeftBottom = radius.BottomLeft + offsetLeft;
            }

            var topLeft = new Point(marginLeftTop, 0);
            var topRight = new Point(rectangle.Width - marginRightTop, 0);
            var rightTop = new Point(rectangle.Width, marginTopRight);
            var rightBottom = new Point(rectangle.Width, rectangle.Height - marginBottomRight);
            var bottomRight = new Point(rectangle.Width - marginRightBottom, rectangle.Height);
            var bottomLeft = new Point(marginLeftBottom, rectangle.Height);
            var leftBottom = new Point(0, rectangle.Height - marginBottomLeft);
            var leftTop = new Point(0, marginTopLeft);

            // So background extends half way into border on each side (otherwise the same antialiasing artifact happens when background matches control).
            topLeft.Y += offsetTop;
            topRight.Y += offsetTop;
            rightTop.X -= offsetRight;
            rightBottom.X -= offsetRight;
            bottomRight.Y -= offsetBottom;
            bottomLeft.Y -= offsetBottom;
            leftBottom.X += offsetLeft;
            leftTop.X += offsetLeft;

            // Handle overlapping points

            if (topLeft.X > topRight.X)
                topLeft.X = topRight.X = marginLeftTop / (marginLeftTop + marginRightTop) * rectangle.Width;

            if (rightTop.Y > rightBottom.Y)
                rightTop.Y = rightBottom.Y = marginTopRight / (marginTopRight + marginBottomRight) * rectangle.Height;

            if (bottomRight.X < bottomLeft.X)
                bottomRight.X = bottomLeft.X = marginLeftBottom / (marginLeftBottom + marginRightBottom) * rectangle.Width;

            if (leftBottom.Y < leftTop.Y)
                leftBottom.Y = leftTop.Y = marginTopLeft / (marginTopLeft + marginBottomLeft) * rectangle.Height;

            var offset = new Vector(rectangle.TopLeft.X, rectangle.TopLeft.Y);
            topLeft += offset;
            topRight += offset;
            rightTop += offset;
            rightBottom += offset;
            bottomRight += offset;
            bottomLeft += offset;
            leftBottom += offset;
            leftTop += offset;

            // Draw
            context.BeginFigure(topLeft, true, true);
            AddCorner(context, topRight, rightTop);
            AddCorner(context, rightBottom, bottomRight);
            AddCorner(context, bottomLeft, leftBottom);
            AddCorner(context, leftTop, topLeft);
        }

        private static void AddCorner(StreamGeometryContext context, Point pointFrom, Point pointTo)
        {
            context.LineTo(pointFrom, true, false);

            var width = Math.Abs(pointFrom.X - pointTo.X);
            var height = Math.Abs(pointFrom.Y - pointTo.Y);
            if (!IsZero(width) || !IsZero(height))
                context.ArcTo(pointTo, new Size(width, height), 0, false, SweepDirection.Clockwise, true, false);
        }

        private static bool IsZero(double value)
        {
            return Math.Abs(value) < 10.0 * 2.2204460492503131e-016;
        }
    }
}