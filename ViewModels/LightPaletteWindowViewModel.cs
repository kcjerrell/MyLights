using MyLights.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.ViewModels
{
    public class LightPaletteWindowViewModel : INotifyPropertyChanged
    {
        public LightPaletteWindowViewModel()
        {
            //var lb = new RestLightBridge();
            var loc = new Locator();
            this.Lights = loc.LightVMs;
        }

        public ObservableCollection<LightViewModel> Lights { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
