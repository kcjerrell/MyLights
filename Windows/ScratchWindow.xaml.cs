using MyLights.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ScratchWindow.xaml
    /// </summary>
    public partial class ScratchWindow : Window
    {
        public ScratchWindow()
        {
            InitializeComponent();
        }
    }

    public class ScratchWindowViewModel : INotifyPropertyChanged
    {


        public double H { get; set; }
        public double S { get; set; }
        public double V { get; set; }
        public HSV HSV { get; set; }
        public Color Color { get; set; }

        public double X { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

}
