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
            RightViewModel = new LibraryViewModel();

            RunTestCommand = new RelayCommand((p) => RunTest());
            SequenceCommand = new RelayCommand((p) => Sequence());
        }

        bool inSequence;
        private async void Sequence()
        {
            if (inSequence)
            {
                inSequence = false;
                return;
            }

            inSequence = true;

            foreach (var light in App.Current.Locator.LightVMs)
            {
                StartSequence(light);
            }
        }

        private static Random rand = new Random();
        private async void StartSequence(LightVM light)
        {
            double hMin = 0.0;
            double hMax = 0.1;
            double sMin = 0.8;
            double sMax = 1.0;
            double vMin = 0.8;
            double vMax = 1.0;
            double tMin = 1.0;
            double tMax = 3.0;

            double h, s, v, h0, s0, v0, h1, s1, v1, t, p;

            DateTime start, stop;

            h1 = rand.NextDouble() * 0.1;
            s1 = .8;
            v1 = .8;

            while (inSequence)
            {
                h0 = h1;
                s0 = s1;
                v0 = v1;

                h1 = rand.NextDouble() * 0.1;
                s1 = rand.NextDouble() * 0.3 + .7;
                v1 = rand.NextDouble() * 0.7 + 0.3;

                t = rand.NextDouble() * (tMax - tMin) + tMin;

                start = DateTime.Now;
                stop = start + TimeSpan.FromSeconds(t);

                while(DateTime.Now < stop)
                {
                    p = (DateTime.Now - start).TotalSeconds / t;
                    h = (h1 - h0) * p + h0;
                    s = (s1 - s0) * p + s0;
                    v = (v1 - v0) * p + v0;

                    light.HSV = new HSV(h, s, v);
                    await Task.Delay(50);
                }
            }
        }

        private async void RunTest()
        {
            var lights = App.Current.Locator.LightVMs;
            foreach (var light in lights)
            {
                light.Power = false;
                light.HSV = new HSV(0, 0, 1);
                await Task.Delay(250);

                light.Power = true;
                await Task.Delay(250);

                light.HSV = new HSV(1, 1, 1);
                await Task.Delay(500);

                light.HSV = new HSV(0.5, .7, .8);
                await Task.Delay(500);

                light.HSV = new HSV(0.8, .3, 1);
                await Task.Delay(500);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public object LeftViewModel { get; set; }

        public object RightViewModel { get; set; }

        public string Error { get; set; } = string.Empty;

        public RelayCommand RunTestCommand { get; }
        public RelayCommand SequenceCommand { get; }

    }
}
