using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyLights.LightMods
{
    interface ILightMod
    {
        public string Name { get; }
        public ImageSource Icon { get; }
        public RelayCommand<LightViewModel> ExecuteCommand { get; }
    }
}
