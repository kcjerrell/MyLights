using MyLights.Util;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MyLights.Models
{
    [DoNotNotify]
    public class Light : INotifyPropertyChanged
    {
        protected Light()
        {

        }

        public static Light FromJson(JsonBulb jBulb)
        {
            string index = jBulb.index.ToString();

            var light = new Light
            {
                Index = jBulb.index,
                Name = jBulb.name,
                color = new DpsColor(index, jBulb.color),
                mode = new DpsMode(index, jBulb.mode),
                power = new DpsPower(index, jBulb.power),
                brightness = new DpsBrightness(index, jBulb.brightness),
                warmth = new DpsWarmth(index, jBulb.colortemp)
            };

            light.WireEvents();
            return light;
        }

        private void WireEvents()
        {
            color.Updated += Dps_Updated;
            mode.Updated += Dps_Updated;
            power.Updated += Dps_Updated;
            brightness.Updated += Dps_Updated;
            warmth.Updated += Dps_Updated;
        }

        private void Dps_Updated(object sender, PropertyChangedEventArgs e)
        {
            string property = e.PropertyName.Capitalize();
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        protected DpsColor      color;
        protected DpsMode       mode;
        protected DpsPower      power;
        protected DpsBrightness brightness;
        protected DpsWarmth     warmth;

        public event PropertyChangedEventHandler PropertyChanged;

        [DoNotNotify]
        public int Index { get; private set; }
        [DoNotNotify]
        public string Name { get; private set; }
        public HSV Color { get => color.Value; }
        public bool Power { get => power.Value; }
        public string Mode { get => mode.Value; }
        public double Brightness { get => brightness.Value; }
        public double Warmth { get => warmth.Value; }

        public void SetColor(HSV value)
        {
            color.Set(value);
        }

        public void SetPower(bool value)
        {
            power.Set(value);
        }

        public void SetMode(string value)
        {
            mode.Set(value);
        }
    }
}
