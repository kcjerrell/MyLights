using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyLights.Views
{
    public class AutoGrid : Grid
    {
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            OnLayoutPropertyChanged((FrameworkElement)visualAdded);

        }

        private void OnLayoutPropertyChanged(FrameworkElement element)
        {
            if (Grid.GetRow(element) >= RowDefinitions.Count)
                RowDefinitions.Add(new RowDefinition());

            if (Grid.GetColumn(element) >= RowDefinitions.Count)
                ColumnDefinitions.Add(new ColumnDefinition());
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            //throw new Exception($"{Grid.GetRow(Children[1])} {AutoGrid.GetRow(Children[1])}");            
            return base.ArrangeOverride(arrangeSize);
        }

        static AutoGrid()
        {
            Grid.RowProperty.OverrideMetadata(typeof(AutoGrid), new FrameworkPropertyMetadata(0, LayoutPropertyChanged));
            Grid.ColumnProperty.OverrideMetadata(typeof(AutoGrid), new FrameworkPropertyMetadata(0, LayoutPropertyChanged));
        }

        private static void LayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new Exception("HEYYY");

            if (d is FrameworkElement element && element.Parent is AutoGrid ag)
            {
                ag.OnLayoutPropertyChanged(element);
            }
        }

    }
}
