using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyLights.LightMods
{
    public class Blinker : ILightMod
    {
        public Blinker()
        {
            ExecuteCommand = new RelayCommand<LightViewModel>(lvm => Execute(lvm));
        }

        public string Name => "Blinker";
        public RelayCommand<LightViewModel> ExecuteCommand { get; }
        public ImageSource Icon { get; } = new BitmapImage(new Uri("Puzzles-256.png"));

        private void Execute(LightViewModel lvm)
        {
            
        }
    }
}
