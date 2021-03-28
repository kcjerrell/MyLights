using MyLights.Views;
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
using System.Windows.Shapes;

namespace MyLights
{
    /// <summary>
    /// Interaction logic for LightPaletteWindow.xaml
    /// </summary>
    public partial class LightPaletteWindow : Window
    {
        public LightPaletteWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            this.Background = new SolidColorBrush(await ColorPickerPopup.GetColor(placementTarget: button));
        }
    }
}
