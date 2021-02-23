using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyLights
{
    public class TreeLampBase : Control
    {
        public TreeLampBase()
        {
            Lamps = new UIElementCollection(this, this);
        }

        public UIElementCollection Lamps
        {
            get { return (UIElementCollection)GetValue(LampsProperty); }
            set { SetValue(LampsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Lamps.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LampsProperty =
            DependencyProperty.Register("Lamps", typeof(UIElementCollection), typeof(TreeLampBase), 
                new PropertyMetadata(null));

        public double RealHeight
        {
            get { return (double)GetValue(RealHeightProperty); }
            set { SetValue(RealHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RealHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RealHeightProperty =
            DependencyProperty.Register("RealHeight", typeof(double), typeof(TreeLampBase), new PropertyMetadata(0));

        public double RealWidth
        {
            get { return (double)GetValue(RealWidthProperty); }
            set { SetValue(RealWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RealWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RealWidthProperty =
            DependencyProperty.Register("RealWidth", typeof(double), typeof(TreeLampBase), new PropertyMetadata(0));

        public ObservableCollection<Point> AttachPositions { get; set; }

        public int AttachPositionsCount { get; set; }
    }
}
