using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace MyLights.Util
{
    public class ItemClipBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            AssociatedObject.SizeChanged += AssociatedObject_SizeChanged;
            AssociatedObject.Clip = new RectangleGeometry();
            UpdateClip();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SizeChanged -= AssociatedObject_SizeChanged;
        }

        private void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateClip();
        }

        private void UpdateClip()
        {
            // It doesn't matter if min is actually min or if they are reversed
            // just make sure value is in between

            double minBound = Math.Min(ValueMin, ValueMax);
            double maxBound = Math.Max(ValueMin, ValueMax);

            if (minBound <= Value && Value <= maxBound && AssociatedObject != null)
            {
                var clip = (RectangleGeometry)AssociatedObject.Clip;
                double w = AssociatedObject.ActualWidth;
                double h = AssociatedObject.ActualHeight;
                double r = (Value - ValueMin) / (ValueMax - ValueMin);

                clip.Rect = FillDirection switch
                {
                    FillDirection.Left => new Rect(w - (r * w), 0, r * w, h),
                    FillDirection.Up => new Rect(0, h - (r * h), w, r * h),
                    FillDirection.Right => new Rect(0, 0, r * w, h),
                    FillDirection.Down => new Rect(0, 0, w, r * h),
                    _ => new Rect(),
                };
            }
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ItemClipBehavior),
                new PropertyMetadata(0.0, (s, e) => ((ItemClipBehavior)s).UpdateClip()));

        public double ValueMin
        {
            get { return (double)GetValue(ValueMinProperty); }
            set { SetValue(ValueMinProperty, value); }
        }

        public static readonly DependencyProperty ValueMinProperty =
            DependencyProperty.Register("ValueMin", typeof(double), typeof(ItemClipBehavior),
                new PropertyMetadata(0.0, (s, e) => ((ItemClipBehavior)s).UpdateClip()));

        public double ValueMax
        {
            get { return (double)GetValue(ValueMaxProperty); }
            set { SetValue(ValueMaxProperty, value); }
        }

        public static readonly DependencyProperty ValueMaxProperty =
            DependencyProperty.Register("ValueMax", typeof(double), typeof(ItemClipBehavior),
                new PropertyMetadata(100.0, (s, e) => ((ItemClipBehavior)s).UpdateClip()));

        public FillDirection FillDirection
        {
            get { return (FillDirection)GetValue(FillDirectionProperty); }
            set { SetValue(FillDirectionProperty, value); }
        }

        public static readonly DependencyProperty FillDirectionProperty =
            DependencyProperty.Register("FillDirection", typeof(FillDirection), typeof(ItemClipBehavior),
                new PropertyMetadata(FillDirection.Up, (s, e) => ((ItemClipBehavior)s).UpdateClip()));
    }

    public enum FillDirection
    {
        Left,
        Up,
        Right,
        Down
    }
}
