using MyLights.Models;
using MyLights.Util;
using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Windows.ViewModels
{
    public class AnotherLightPanelViewModel : INotifyPropertyChanged
    {
        public AnotherLightPanelViewModel()
        {
            // var lb = new LightUdp.UdpLightBridge();
            // var lb = App.Current.Locator.LightBridge;
            var lb = App.Current.LightBridge;
            LightVMs = lb.LightVMs;
        }

        public ObservableCollection<LightViewModel> LightVMs { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
