using MyLights.Util;
using MyLights.ViewModels;
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

            light.CoerceLightMode();
            light.WireEvents();
            return light;
        }

        private void CoerceLightMode()
        {
            LightModes oldValue = _lightMode;

            if (!Power)
                _lightMode = LightModes.Off;

            else if (Mode == "color")
                _lightMode = LightModes.Color;

            else if (Mode == "white")
                _lightMode = LightModes.White;

            if (_lightMode != oldValue)
                Dps_Updated(this, new PropertyChangedEventArgs("LightMode"));
        }

        public void SetLightMode(LightModes lightMode)
        {
            _lightMode = lightMode;

            if (lightMode == LightModes.Off)
            {
                power.Set(false);
            }

            else if (lightMode == LightModes.Color)
            {
                mode.Set("color");

                if (!Power)
                    power.Set(true);
            }

            else if (lightMode == LightModes.White)
            {
                mode.Set("white");

                if (!Power)
                    power.Set(true);
            }

            Dps_Updated(this, new PropertyChangedEventArgs("LightMode"));
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

            CoerceLightMode();
        }

        protected DpsColor color;
        protected DpsMode mode;
        protected DpsPower power;
        protected DpsBrightness brightness;
        protected DpsWarmth warmth;
        private LightModes _lightMode;

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
        public LightModes LightMode
        {
            get => _lightMode;
            private set
            {
                if (_lightMode != value)
                {
                    SetLightMode(value);
                }
            }
        }


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
