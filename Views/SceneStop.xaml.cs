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
    /// Interaction logic for SceneStop.xaml
    /// </summary>
    public partial class SceneStop : UserControl
    {
        public SceneStop()
        {
            InitializeComponent();
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var handler = DeleteRequested;
            handler?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler DeleteRequested;

    }
}
