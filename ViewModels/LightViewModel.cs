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

        public double Brightness
        {
            get => Light.Brightness;
            set
            {
                Light.SetBrightness(value);
            }
        }

        public double ColorTemp
        {
            get => Light.ColorTemp;
            set
            {
                Light.SetColorTemp(value);
            }
        }

        public HSV Color
        {
            get => Light.Color;
            set
            {
                UpdateColor(value.H, value.S, value.V);
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

        public int Index { get => 0; }

        public override string ToString()
        {
            return $"(LVM: {Name}-{(Power ? "on" : "off")}-{Mode}-{Color}";
        }

        //[DependsOn("Mode", "Power")]
        //public LightModes LightMode
        //{
        //    get
        //    {
        //        if (Light.Power && Light.Mode.ToLower() == "color")
        //            return LightModes.Color;
        //        else if (Light.Power && Light.Mode.ToLower() == "white")
        //            return LightModes.White;
        //        else
        //            return LightModes.Off;
        //    }
        //    set
        //    {
        //        if (value == LightModes.Off)
        //        {
        //            Power = false;
        //        }
        //        else if (value == LightModes.Color)
        //        {
        //            Power = true;
        //            Mode = "color";
        //        }
        //        else if (value == LightModes.White)
        //        {
        //            Power = true;
        //            Mode = "white";
        //        }
        //    }
        //}

        //private LightModes _lightMode;
        //public LightModes LightMode
        //{
        //    get
        //    {
        //        if (!Power)
        //            return LightModes.Off;
        //        else if (Mode == "color")
        //            return LightModes.Color;
        //        else
        //            return LightModes.White;
        //    }
        //    set
        //    {
        //        if (value == LightModes.Off)
        //        {
        //            Power = false;
        //        }
        //        else
        //        {
        //            if (value == LightModes.Color)
        //                Mode = "color";
        //            else if (value == LightModes.White)
        //                Mode = "white";
        //
        //
        //            Power = true;
        //        }
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public enum LightModes
    {       
        Color,
        White
    }

    public class ModeOptions
    {
        public static List<string> ListOptions()
        {
            var options = new List<string>();
            options.Add("color");
            options.Add("white");
            return options;
        }
    }
}
