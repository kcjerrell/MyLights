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
        public Light(ILightPropertiesProvider propertiesProvider)
        {
            Index = propertiesProvider.Index;
            Name = propertiesProvider.Name;

            //I'm trying to decide if I should keep Power and Mode as seperate properties 
            //Like they are on the bulb itself, or if I should combine them in the more 
            //convenient LightModes form (LightMode = Off, Color, or White);
            //And the reason for that is because it bugs the fuck out of me when I say 
            //"turn lights on" and then I have to say "turn lights white"; Typically when I turn
            //them on, I have "white" or "color" in mind anyway, right? Although perhaps it should
            //be toggleable without having to track color/white. Hmm....
            //I'll just keep what I'm doing: have the binding property as it is on the device
            //(power [true/false] and mode [white/color]), have both of those accesible here, and
            //a third property that access the other two. so all this commentary and no change. :)

            power = propertiesProvider.PowerProperty;
            color = propertiesProvider.ColorProperty;
            mode = propertiesProvider.ModeProperty;
            brightness = propertiesProvider.BrightnessProperty;
            colorTemp = propertiesProvider.ColorTempProperty;

            DetermineLightMode();
            WireEvents();

        }

        private void DetermineLightMode()
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
            colorTemp.Updated += Dps_Updated;
        }

        private void Dps_Updated(object sender, PropertyChangedEventArgs e)
        {
            string property = e.PropertyName.Capitalize();
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(property));

            DetermineLightMode();
        }

        protected IDeviceProperty<HSV> color;
        protected IDeviceProperty<string> mode;
        protected IDeviceProperty<bool> power;
        protected IDeviceProperty<double> brightness;
        protected IDeviceProperty<double> colorTemp;
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
        public double Warmth { get => colorTemp.Value; }
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
