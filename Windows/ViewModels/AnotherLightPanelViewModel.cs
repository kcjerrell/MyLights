using MyLights.LightMods;
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
using System.Windows;

namespace MyLights.Windows.ViewModels
{
    public class AnotherLightPanelViewModel : INotifyPropertyChanged
    {
        public AnotherLightPanelViewModel()
        {
            LightVMs = Locator.Get.LightVMs;
            linker = new LightViewModelLinker(LightVMs);
            Library = Locator.Get.Library;
        }

        private LightViewModelLinker linker;

        public ObservableCollection<LightViewModel> LightVMs { get; set; }
        public ILightEffect SelectedMultiLightEffect { get; set; }

        public LibraryViewModel Library { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
