using MyLights.Util;
using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MyLights.Windows
{
    /// <summary>
    /// Interaction logic for DevConsole.xaml
    /// </summary>
    public partial class DevConsole : Window
    {
        public DevConsole()
        {
            InitializeComponent();

            logItemsControl.ItemsSource = Logger.LogItems;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var lvms = (ObservableCollection<LightViewModel>) lvmsItemsControl.ItemsSource;

                string input = cliTextBox.Text;

                if (double.TryParse(input, out double value))
                {
                    lvms[0].H = value;
                }
            }
        }
    }
}
