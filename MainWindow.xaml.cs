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
using Flurl;
using Flurl.Http;
using MyLights.ViewModels;
using Xceed.Wpf.Toolkit;

namespace MyLights
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;


        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            colorPicker.AvailableColors.Clear();

            var colors = new ColorItem[] {
                new ColorItem(Colors.Red, "Red"),
                new ColorItem(Colors.Orange, "Orange"),
                new ColorItem(Colors.Yellow, "Yellow"),
                new ColorItem(Colors.Chartreuse, "Chartreuse"),
                new ColorItem(Colors.Green, "Green"),
                new ColorItem(Colors.Turquoise, "Turquoise"),
                new ColorItem(Colors.Blue, "Blue"),
                new ColorItem(Colors.RoyalBlue, "RoyalBlue"),
                new ColorItem(Colors.Purple, "Purple"),
                new ColorItem(Colors.Magenta, "Magenta"),
                new ColorItem(Colors.White, "White"),
            };

            foreach (var c in colors)
            {
                colorPicker.AvailableColors.Add(c);                
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void LightPanel_FlyoutRequest(object sender, Views.FlyoutRequestEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show(((LightViewModel)e.Source.DataContext).Name);
        }
    }

}
