using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyLights.Views
{
    public class TwoColumnPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            double wMax = 0;
            double hMax = 0;

            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);

                if (child.DesiredSize.Width > wMax)
                    wMax = child.DesiredSize.Width;

                if (child.DesiredSize.Height > hMax)
                    hMax = child.DesiredSize.Height;
            }

            Size childSize = new Size(wMax, hMax);

            int columns = 2;
            int rows = (Children.Count + 1) / columns;

            return new Size(childSize.Width * columns, childSize.Height * rows);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double childSide = finalSize.Width / 2.0;
            int i = 0;

            foreach (UIElement child in Children)
            {
                int col = i % 2;
                int row = i / 2;

                double x = childSide * col;
                double y = childSide * row;

                child.Arrange(new Rect(x, y, childSide, childSide));

                i += 1;
            }

            return new Size(childSide * 2.0, childSide * (Children.Count + 1) / 2);
        }

    }
}
