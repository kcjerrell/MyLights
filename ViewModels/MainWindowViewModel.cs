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

            RunTestCommand = new RelayCommand((p) => RunTest());
        }

        private async void RunTest()
        {
            var lights = App.Current.Locator.LightVMs;
            foreach (var light in lights)
            {
                light.Power = false;
                light.Color = Color.FromRgb(255, 255, 255);
                await Task.Delay(250);

                light.Power = true;
                await Task.Delay(250);

                light.Color = Color.FromRgb(255, 0, 0);
                await Task.Delay(500);

                light.Color = Color.FromRgb(0, 255, 255);
                await Task.Delay(500);

                light.Color = Color.FromRgb(255, 255, 255);
                await Task.Delay(500);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public object LeftViewModel { get; set; }

        public object RightViewModel { get; set; }

        public string Error { get; set; } = string.Empty;

        public RelayCommand RunTestCommand { get; }

    }
}
