using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MyLights.LightMods
{
    [DoNotNotify]
    public abstract class PluginSetting : INotifyPropertyChanged
    {
        public string Name { get; init; }

        protected void Notify()
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs("Value"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    [DoNotNotify]
    public class NumericPluginSetting : PluginSetting
    {
        public NumericPluginSetting(double initialValue, double minValue, double maxValue, string name)
        {
            this.Name = name;
            this.MinValue = MinValue;
            this.MaxValue = MaxValue;
            this.Value = initialValue;
        }

        public double MinValue { get; set; }
        public double MaxValue { get; set; }

        private double _value;
        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                Notify();
            }
        }
    }
}
