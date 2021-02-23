using MyLights.Models;
using MyLights.Util;
using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LightVM = MyLights.ViewModels.LightViewModel;

namespace MyLights.Etc
{
    public class Junk
    {

        public Junk()
        {
            FunCommand = new RelayCommand((p) => Fun());
            BlinkCommand = new RelayCommand((p) => Blink());
        }
        public ObservableCollection<LightVM> LightVMs { get; private set; } = new ObservableCollection<LightVM>();
        private List<Light> lights = new List<Light>();
        public RelayCommand FunCommand { get; }
        public RelayCommand BlinkCommand { get; }
        private async void Blink()
        {
            int n = lights.Count;

            for (int i = 0; i < n; i++)
            {
                lights[i].SetPower(false);
                await Task.Delay(200);
            }

            for (int i = 0; i < n; i++)
            {
                for (int i2 = 0; i2 < i; i2++)
                {
                    lights[i2].SetPower(true);
                }

                await Task.Delay(500);

                for (int i2 = 0; i2 < i; i2++)
                {
                    lights[i2].SetPower(false);
                }

                await Task.Delay(500);
            }

            for (int i = 0; i < n; i++)
            {
                lights[i].SetPower(true);
                await Task.Delay(200);
            }

        }

        private bool alreadyFun = false;
        private async void Fun()
        {
            if (alreadyFun)
            {
                alreadyFun = false;
                return;
            }

            //var fades = new RandomColorFader[5];
            //for (int i = 0; i < 5; i++)
            //{
            //    fades[i] = new RandomColorFader();
            //}

            int interval = 100;

            var fade = new RandomColorFader(interval);

            alreadyFun = true;
            //int i = 0;

            while (alreadyFun)
            {
                Color c = fade.Next();
                for (int bi = 0; bi < lights.Count; bi++)
                {
                    lights[bi].SetColor(c);
                }

                await Task.Delay(interval);
                //i += 1;
            }
        }


    }

    public class RandomColorFader
    {

        static Random rand = new Random();

        public RandomColorFader(int interval)
        {
            stepsMin = (int)(5 * 1000.0 / interval);
            stepsMax = (int)(60 * 1000.0 / interval);
            SetNewTarget();
        }

        private void SetNewTarget()
        {
            SetNewTarget(RandHSV());
        }

        private void SetNewTarget(HSV startFrom)
        {
            from = startFrom;
            to = RandHSV();

            prog = 0;
            steps = rand.Next(stepsMin, stepsMax);
        }

        public Color Next()
        {
            if (prog >= steps)
            {
                SetNewTarget(to);
            }

            double x = 1.0 * prog / steps;

            byte h = (byte)Math.Round((to.H - from.H) * x + from.H);
            byte s = (byte)Math.Round((to.S - from.S) * x + from.S);
            byte v = (byte)Math.Round((to.V - from.V) * x + from.V);

            prog += 1;

            return Helpers.HsvToColor(h, s, v);
        }

        HSV from;
        HSV to;

        int steps;
        int prog;

        int stepsMin;
        int stepsMax;

        private Color RandColor()
        {
            double h = rand.NextDouble() * 360;
            double s = rand.NextDouble() / 2.0 + .5;

            int r, g, b;

            Helpers.HsvToRgb(h, s, 1.0, out r, out g, out b);

            return Color.FromRgb((byte)r, (byte)g, (byte)b);
        }

        private HSV RandHSV()
        {
            double h = rand.NextDouble();
            double s = rand.NextDouble() / 2.0 + .5;
            double v = rand.NextDouble() / 3.0 + 2.0 / 3.0;

            return new HSV(h * 360.0, s, v);
        }
    }
}
