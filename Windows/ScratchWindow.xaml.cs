using MyLights.Models;
using MyLights.Util;
using MyLights.ViewModels;
using MyLights.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ScratchWindowViewModel()
        {
            AddCommand = new RelayCommand((p) =>
            {
                Modulators.Add(new Modulator());
            });

            AddCommand.Execute(null);
        }
        public RelayCommand AddCommand { get; private set; }
        public ObservableCollection<Modulator> Modulators { get; } = new ObservableCollection<Modulator>();
        //public ObservableCollection<LightViewModel> Lights = (new RestLightBridge()).LightVMs;
        public ObservableCollection<LightViewModel> Lights = App.Current.Locator.LightVMs;
        public double H { get; set; }
        public double S { get; set; }
        public double V { get; set; }
        public HSV HSV { get; set; }
        public Color Color { get; set; }

        public double X { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

}
