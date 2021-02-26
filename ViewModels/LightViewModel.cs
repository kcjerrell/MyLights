using MyLights.Models;
using MyLights.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyLights.ViewModels
{
    public class LightViewModel : INotifyPropertyChanged
    {

        public LightViewModel(Light light)
        {
            this.light = light;

            Name = light.Name;
            power = light.Power;
            h = light.Color.H;
            s = light.Color.S;
            v = light.Color.V;
            color = HSV.ToColor();
        }

        private bool power;
        private Light light;
        private Color color;
        private double h, s, v;

        public string Name { get; }
        public bool Power
        {
            get => power;
            set
            {
                power = value;
                light.SetPower(value);
            }
        }

        public Color Color
        { 
            get => color;
            set
            {
                UpdateColor(value);
            }
        }

        public HSV HSV
        {
            get => new HSV(h, s, v);
            set
            {
                UpdateColor(value);
            }
        }

        public double H
        {
            get => this.h;
            set
            {
                UpdateColor(value, this.s, this.v);
            }
        }

        public double S
        {
            get => this.s;
            set
            {
                UpdateColor(this.h, value, this.v);
            }
        }

        public double V
        {
            get => this.v;
            set
            {
                UpdateColor(this.h, this.s, value);
            }
        }

        private void UpdateColor(double h, double s, double v)
        {
            UpdateColor(new HSV(h, s, v));
        }

        private void UpdateColor(HSV hsv)
        {
            this.h = hsv.H;
            this.s = hsv.S;
            this.v = hsv.V;
            this.color = hsv.ToColor();
            light.SetColor(hsv);
        }

        private void UpdateColor(Color color)
        {
            this.color = color;
            var hsv = HSV.FromColor(color);
            this.h = hsv.H;
            this.s = hsv.S;
            this.v = hsv.V;
            light.SetColor(hsv);
        }

        public int Index { get => light.Index; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
