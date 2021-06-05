using MyLights.Bridges;
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
        private string name;

        public LightViewModel(Light light)
        {
            this.Light = light;
            light.PropertyChanged += Light_PropertyChanged;

            Name = KnownDevices.GetName(light.Id);
        }

        public Light Light { get; init; }
        public string Name
        {
            get => name;
            set
            {
                name = value;
                KnownDevices.UpdateName(Light.Id, value);
            }
        }
        public LightMode Mode
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
        [DependsOn("Color")]
        public double H
        {
            get => Color.H; // this.hsv.H;
            set
            {
                UpdateColor(h: value);
            }
        }
        [DependsOn("Color")]
        public double S
        {
            get => Color.S; // this.hsv.S;
            set
            {
                UpdateColor(s: value);
            }
        }
        [DependsOn("Color")]
        public double V
        {
            get => Color.V; // this.hsv.V;
            set
            {
                UpdateColor(v: value);
            }
        }

        public bool IsSelected { get; set; }
        private bool isLinked;
        public bool IsLinked { get => isLinked; set => isLinked = SetLink(this, value); }

        private void Light_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, e);

            if (e.PropertyName == "Color")
            {
                handler?.Invoke(this, new PropertyChangedEventArgs("H"));
                handler?.Invoke(this, new PropertyChangedEventArgs("S"));
                handler?.Invoke(this, new PropertyChangedEventArgs("V"));
            }
        }

        private void UpdateColor(double h = -1.0, double s = -1.0, double v = -1.0)
        {
            var hsv = Light.Color;

            if (h == -1)
                h = hsv.H;
            if (s == -1)
                s = hsv.S;
            if (v == -1)
                v = hsv.V;

            Light.SetColor(new HSV(h, s, v));
        }

        public override string ToString()
        {
            return $"(LVM: {Name}-{(Power ? "on" : "off")}-{Mode}-{Color}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Probably a bad idea getting all static here, but I'm gonna do it anyway

        static List<LightViewModel> linkedLights = new();

        private static bool SetLink(LightViewModel light, bool value)
        {
            if (value && !light.isLinked)
            {
                linkedLights.Add(light);
                light.PropertyChanged += LinkedLight_PropertyChanged;
            }
            else if (!value && light.isLinked)
            {
                linkedLights.Remove(light);
                light.PropertyChanged -= LinkedLight_PropertyChanged;
            }

            return value;
        }

        private static void LinkedLight_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            LightViewModel source = (LightViewModel)sender;
            Action<LightViewModel> linkedSet = e.PropertyName switch
            {
                nameof(LightViewModel.Power) => (LightViewModel target) => target.Power = source.Power,
                nameof(LightViewModel.Mode) => (LightViewModel target) => target.Mode = source.Mode,
                nameof(LightViewModel.Brightness) => (LightViewModel target) => target.Brightness = source.Brightness,
                nameof(LightViewModel.ColorTemp) => (LightViewModel target) => target.ColorTemp = source.ColorTemp,
                nameof(LightViewModel.Color) => (LightViewModel target) => target.Color = source.Color,
                _ => null
            };

            if (linkedSet == null)
                return;

            foreach (LightViewModel lvm in linkedLights.Without(source))
            {
                linkedSet(lvm);
            }
        }
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