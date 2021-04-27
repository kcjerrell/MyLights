using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyLights.Controls
{
    public class UniformishWrapPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            (int col, double cw) = CalcChildWidth(availableSize.Width, MinChildWidth, MaxChildWidth);
            double ch = cw / AspectRatio;
                       
            foreach (UIElement child in Children)
            {
                child.Measure(new Size(cw, ch));
            }

            // this is wrong
            double height = ch * (Children.Count / col + 1);

            return new Size(col * cw, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            (int col, double cw) = CalcChildWidth(finalSize.Width, MinChildWidth, MaxChildWidth);
            //double ch = cw / AspectRatio;

            int i = 0;
            double rowHeight = 0;
            double y = 0;
            foreach (UIElement child in Children)
            {
                double x = (i % col) * cw;
                if (x == 0)
                {
                    y += rowHeight;
                    rowHeight = 0;
                }

                double ch = child.DesiredSize.Height;
                rowHeight = Math.Max(rowHeight, ch);
                child.Arrange(new Rect(x, y, cw, ch));
                i += 1;
            }

            // this is wrong
            double height = y + rowHeight;

            return new Size(col * cw, height);
        }

        private static (int col, double cw) CalcChildWidth(double availableWidth, double childMin, double childMax) 
        {
            double minCol = availableWidth / childMin;
            double maxCol = availableWidth / childMax;

            maxCol = Math.Max(Math.Floor(maxCol), 1);

            double childWidth = Math.Max(availableWidth / maxCol, childMin);

            return ((int)maxCol, childWidth);
        }

        public double MaxChildWidth
        {
            get { return (double)GetValue(MaxChildWidthProperty); }
            set { SetValue(MaxChildWidthProperty, value); }
        }

        public static readonly DependencyProperty MaxChildWidthProperty =
            DependencyProperty.Register("MaxChildWidth", typeof(double), typeof(UniformishWrapPanel),
                new FrameworkPropertyMetadata(300.0, FrameworkPropertyMetadataOptions.AffectsArrange 
                    | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double MinChildWidth
        {
            get { return (double)GetValue(MinChildWidthProperty); }
            set { SetValue(MinChildWidthProperty, value); }
        }

        public static readonly DependencyProperty MinChildWidthProperty =
            DependencyProperty.Register("MinChildWidth", typeof(double), typeof(UniformishWrapPanel),
                new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double AspectRatio
        {
            get { return (double)GetValue(AspectRatioProperty); }
            set { SetValue(AspectRatioProperty, value); }
        }

        public static readonly DependencyProperty AspectRatioProperty =
            DependencyProperty.Register("AspectRatio", typeof(double), typeof(UniformishWrapPanel),
                new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsArrange
                    | FrameworkPropertyMetadataOptions.AffectsMeasure));


    }
}
