using Flurl.Http;
using MyLights.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MyLights.Util;
using LightVM = MyLights.ViewModels.LightViewModel;


namespace MyLights.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            LeftViewModel = new LightTrackViewModel();
            RightViewModel = new SceneSelectionViewModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public object LeftViewModel { get; set; }

        public object RightViewModel { get; set; }

        public string Error { get; set; } = string.Empty;

    }
}
