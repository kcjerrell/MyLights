using MyLights.Models;
using MyLights.Util;
using MyLights.ViewModels;
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

namespace MyLights.Controls
{
    /// <summary>
    /// Interaction logic for LightControl.xaml
    /// </summary>
    public partial class LightControl : UserControl
    {
        public LightControl()
        {
            InitializeComponent();

            Loaded += LightControl_Loaded;
        }

        private LightViewModel GetDataContext()
        {
            return (LightViewModel)this.DataContext;
        }

        private void LightControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Locator.IsInDesignMode)
            {
                this.DataContext = Locator.Get.DesignLightVM;
            }
        }

        private void addColorToFavsButton_Click(object sender, RoutedEventArgs e)
        {
            Locator.Get.Library.AddColor(new Models.HSV(xySelector.ValueX, xySelector.ValueY, 1));
        }

        private void colorViewToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            colorWhiteContainer.Visibility = Visibility.Collapsed;
            favesRoot.Visibility = Visibility.Visible;
        }

        private void colorViewToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            colorWhiteContainer.Visibility = Visibility.Visible;
            favesRoot.Visibility = Visibility.Collapsed;
        }

        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            var lb = (ListBox)sender;
            if (e.Key == Key.Delete)
            {
                if (lb.SelectedItem is HSV color)
                {
                    Locator.Get.Library.RemoveColor(color);
                }
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lb = (ListBox)sender;
            var color = (HSV)lb.SelectedItem;

            GetDataContext().Color = color;
        }
    }
}
