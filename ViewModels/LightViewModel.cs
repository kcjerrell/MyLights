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
                UpdateColor(value.H, value.S,  value.V);
            }
        }

        public double H
        {
            get => this.hsv.H;
            set
            {
                UpdateColor(h: value);
            }
        }

        public double S
        {
            get => this.hsv.S;
            set
            {
                UpdateColor(s: value);
            }
        }

        public double V
        {
            get => this.hsv.V;
            set
            {
                UpdateColor(v: value);
            }
        }

        private void UpdateColor(double h = -1.0, double s = -1.0, double v = -1.0)
        {
            if (h == -1)
                h = hsv.H;
            if (s == -1)
                s = hsv.S;
            if (v == -1)
                v = hsv.V;

            hsv = new HSV(h, s, v);

            Light.SetColor(hsv);
        }

        public bool IsSelected { get; set; }

        public int Index { get => Light.Index; }

        public LightModes LightMode
        {
            get
            {
                if (Light.Power && Light.Mode.ToLower() == "color")
                    return LightModes.Color;
                else if (Light.Power && Light.Mode.ToLower() == "white")
                    return LightModes.White;
                else
                    return LightModes.Off;
            }
            set
            {
                if (value == LightModes.Off)
                {
                    Power = false;
                }
                else if (value == LightModes.Color)
                {
                    Power = true;
                    Mode = "color";
                }
                else if (value == LightModes.White)
                {
                    Power = true;
                    Mode = "white";
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public enum LightModes
    {
        Off,
        Color,
        White
    }
}
