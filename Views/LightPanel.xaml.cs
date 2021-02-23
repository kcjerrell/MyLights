using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyLights.Views
{
    /// <summary>
    /// Interaction logic for LightTrack.xaml
    /// </summary>
    public partial class LightPanel : UserControl
    {
        public LightPanel()
        {
            InitializeComponent();
            AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(SomeMouseLeftButtonUp), true);
        }


        private void SomeMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (clicked_on != null && clicked_on.IsMouseOver)
            {
                var handler = FlyoutRequest;
                handler?.Invoke(this, new FlyoutRequestEventArgs() { Source = clicked_on });
            }
            clicked_on = null;
        }

        private FrameworkElement clicked_on;
        private void lightItemRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            clicked_on = (FrameworkElement)sender;
        }

        public event FlyoutRequestEventHandler FlyoutRequest;

    }

    public delegate void FlyoutRequestEventHandler(object sender, FlyoutRequestEventArgs e);

    public class FlyoutRequestEventArgs : EventArgs
    {
        public FrameworkElement Source { get; set; }
    }
}
