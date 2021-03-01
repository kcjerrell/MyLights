using MyLights.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MyLights.ViewModels
{
    public class LightGroupViewModel : INotifyPropertyChanged
    {
        private List<LightViewModel> lightvms;
        private LightGroup lightGroup;

        public LightGroupViewModel(List<LightViewModel> lightVms)
        {
            this.lightvms = lightVms;

            foreach (var lvm in lightvms)
            {
                lvm.Group = this;              
            }

            var lights = from lvm in lightvms
                         select lvm.Light;

            lightGroup = new LightGroup(lights.ToList());

        }
        public HSV HSV { get; private set; }
        public bool Power { get; private set; }
        public string Mode { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void Ungroup()
        {
            foreach (var lvm in lightvms)
            {
                lvm.Group = null;
            }

            lightGroup.Ungroup();
        }

        internal void SetMode(string value)
        {
            Mode = value;
            lightGroup.SetMode(value);
            foreach (var lvm in lightvms)
            {
                lvm.Mode = value;
            }
        }

        internal void SetColor(HSV hsv)
        {
            HSV = hsv;

            lightGroup.SetColor(hsv);

            foreach (var lvm in lightvms)
            {
                lvm.HSV = hsv;
            }
        }

        internal void SetPower(bool value)
        {
            Power = value;

            lightGroup.SetPower(value);

            foreach(var lvm in lightvms)
            {
                lvm.Power = value;
            }
        }
    }
}