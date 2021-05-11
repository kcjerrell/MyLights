using MyLights.Bridges;
using MyLights.Util;
using MyLights.ViewModels;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace MyLights.Models
{
    [DoNotNotify]
    public class Light : INotifyPropertyChanged
    {
        public Light(ILightPropertiesProvider propertiesProvider)
        {
            // Index = propertiesProvider.Index;
            Id = propertiesProvider.Id;

            power = propertiesProvider.PowerProperty;
            color = propertiesProvider.ColorProperty;
            mode = propertiesProvider.ModeProperty;
            brightness = propertiesProvider.BrightnessProperty;
            colorTemp = propertiesProvider.ColorTempProperty;

            WireEvents();
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
            if (uiDispatcher == null)
                uiDispatcher = App.Current.Dispatcher;

            if (Thread.CurrentThread.ManagedThreadId != uiDispatcher.Thread.ManagedThreadId)
            {
                uiDispatcher.Invoke(() => Dps_Updated(sender, e));
            }
            else
            {
                var handler = PropertyChanged;
                handler?.Invoke(this, e);
            }
        }

        private static Dispatcher uiDispatcher;

        protected IDeviceProperty<HSV> color;
        protected IDeviceProperty<LightMode> mode;
        protected IDeviceProperty<bool> power;
        protected IDeviceProperty<double> brightness;
        protected IDeviceProperty<double> colorTemp;

        public event PropertyChangedEventHandler PropertyChanged;

        [DoNotNotify]
        public string Id { get; private set; }
        public HSV Color { get => color.Value; }
        public bool Power { get => power.Value; }
        public LightMode Mode { get => mode.Value; }
        public double Brightness { get => brightness.Value; }
        public double ColorTemp { get => colorTemp.Value; }

        public void SetColor(HSV value, bool immediate = false)
        {
            color.Set(value, immediate);
        }

        public void SetPower(bool value, bool immediate = false)
        {
            power.Set(value, immediate);
        }

        public void SetMode(LightMode value, bool immediate = false)
        {
            mode.Set(value, immediate);
        }

        public void SetBrightness(double value, bool immediate = false)
        {
            brightness.Set(value, immediate);
        }

        public void SetColorTemp(double value, bool immediate = false)
        {
            colorTemp.Set(value, immediate);
        }
    }
}
