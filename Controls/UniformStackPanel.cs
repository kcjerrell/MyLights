using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyLights.Controls
{
    public class UniformStackPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            double dw = 0;
            double dh = 0;

            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);

                if (IsVertical)
                {
                    dh += child.DesiredSize.Height;
                    dw = Math.Max(child.DesiredSize.Width, dw);
                }
                else
                {
                    dw += child.DesiredSize.Width;
                    dh = Math.Max(child.DesiredSize.Height, dh);
                }
            }

            return new Size(0, 0);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double x = 0, y = 0, cw = 0, ch = 0;

            if (IsVertical)
            {
                cw = finalSize.Width;
                ch = finalSize.Height / Children.Count;
            }
            else
            {
                cw = finalSize.Width / Children.Count;
                ch = finalSize.Height;
            }
                        
            foreach (UIElement child in Children)
            {
                child.Arrange(new Rect(x, y, cw, ch));

                if (IsVertical)
                    y += ch;
                else
                    x += cw;
            }

            return finalSize;
        }

        public bool IsVertical => Orientation == Orientation.Vertical;

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(UniformStackPanel),
                new FrameworkPropertyMetadata(Orientation.Horizontal, 
                    FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsMeasure));

    }
}
