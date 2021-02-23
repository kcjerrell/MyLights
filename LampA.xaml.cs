using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyLights
{
    /// <summary>
    /// Interaction logic for LampA.xaml
    /// </summary>
    public partial class LampA : UserControl
    {
        public LampA()
        {
            InitializeComponent();
        }



        public double RealWidth
        {
            get { return (double)GetValue(RealWidthProperty); }
            set { SetValue(RealWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RealWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RealWidthProperty =
            DependencyProperty.Register("RealWidth", typeof(double), typeof(LampA), new PropertyMetadata(0.0));

        public double RealHeight
        {
            get { return (double)GetValue(RealHeightProperty); }
            set { SetValue(RealHeightProperty, value); }
        }

        public static readonly DependencyProperty RealHeightProperty =
            DependencyProperty.Register("RealHeight", typeof(double), typeof(LampA),
                new PropertyMetadata(0.0, (s, e) => ((LampA)s).OnRealHeightChanged(e)));

        private void OnRealHeightChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        public Point AttachPosition
        {
            get { return (Point)GetValue(AttachPositionProperty); }
            set { SetValue(AttachPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AttachPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AttachPositionProperty =
            DependencyProperty.Register("AttachPosition", typeof(Point), typeof(LampA), new PropertyMetadata(default(Point)));



        public double AttachExtensionMinimum
        {
            get { return (double)GetValue(AttachExtensionMinimumProperty); }
            set { SetValue(AttachExtensionMinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AttachExtensionMinimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AttachExtensionMinimumProperty =
            DependencyProperty.Register("AttachExtensionMinimum", typeof(double), typeof(LampA), new PropertyMetadata(0.0));



        public double AttachExtensionMaximum
        {
            get { return (double)GetValue(AttachExtensionMaximumProperty); }
            set { SetValue(AttachExtensionMaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AttachExtensionMaximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AttachExtensionMaximumProperty =
            DependencyProperty.Register("AttachExtensionMaximum", typeof(double), typeof(LampA), new PropertyMetadata(0.0));


    }
}
