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
            this.Light = light;

            Name = light.Name;
            power = light.Power;
            hsv = light.Color;
            mode = light.Mode;
        }

        private bool power;
        private HSV hsv;
        private string mode;

        public Light Light { get; }
        public string Name { get; }

        public string Mode
        {
            get => mode;
            set
            {
                mode = value;
                // #lightgroup 
                if (Group != null)
                {
                    if (Group.Mode != value)
                        Group.SetMode(value);
                }
                else
                    Light.SetMode(value);
            }
        }

        public bool Power
        {
            get => power;
            set
            {
                power = value;
                // #lightgroup
                if (Group != null)
                {
                    if (Group.Power != value)
                        Group.SetPower(value);
                }
                else
                    Light.SetPower(value);
            }
        }

        public HSV HSV
        {
            get => hsv;
            set
            {
                if (HSV != value)
                {
                    hsv = value;
                    UpdateColor(value);
                }
            }
        }

        public double H
        {
            get => this.hsv.H;
            set
            {
                if (HSV.H != value)
                    UpdateColor(value, hsv.S, hsv.V);
            }
        }

        public double S
        {
            get => this.hsv.S;
            set
            {
                if (hsv.S != value)
                    UpdateColor(hsv.H, value, hsv.V);
            }
        }

        public double V
        {
            get => this.hsv.V;
            set
            {
                if (hsv.V != value)
                    UpdateColor(hsv.H, hsv.S, value);
            }
        }

        private void UpdateColor(HSV hsv)
        {
            // #lightgroup 
            if (Group != null)
            {
                if (Group.HSV != hsv)
                    Group.SetColor(hsv);
            }
            else
                Light.SetColor(hsv);
        }

        private void UpdateColor(double h, double s, double v)
        {
            HSV = new HSV(h, s, v);
        }

        public bool IsSelected { get; set; }
        // #lightgroup 
        public LightGroupViewModel Group { get; set; }

        public int Index { get => Light.Index; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
