using MyLights.Models;
using MyLights.Util;
using PropertyChanged;
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
            this.Light = light;
            light.PropertyChanged += Light_PropertyChanged;

            Name = light.Name;
            hsv = light.Color;
        }

        private void Light_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, e);
        }

        private HSV hsv;

        public Light Light { get; }
        public string Name { get; }

        public string Mode
        {
            get => Light.Mode;
            set
            {
                Light.SetMode(value);
            }
        }

        public bool Power
        {
            get => Light.Power;
            set
            {
                Light.SetPower(value);
            }
        }

        public HSV Color
        {
            get => Light.Color;
            set
            {
                UpdateColor(value);
            }
        }

        public double H
        {
            get => this.hsv.H;
            set
            {
                UpdateColor(value, hsv.S, hsv.V);
            }
        }

        public double S
        {
            get => this.hsv.S;
            set
            {
                UpdateColor(hsv.H, value, hsv.V);
            }
        }

        public double V
        {
            get => this.hsv.V;
            set
            {
                UpdateColor(hsv.H, hsv.S, value);
            }
        }

        private void UpdateColor(HSV hsv)
        {
            Light.SetColor(hsv);
        }

        private void UpdateColor(double h, double s, double v)
        {
            Color = new HSV(h, s, v);
        }

        public bool IsSelected { get; set; }

        public int Index { get => Light.Index; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
