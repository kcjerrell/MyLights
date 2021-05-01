using PropertyChanged;
using System;
using System.ComponentModel;

namespace MyLights.LightMods
{
    [DoNotNotify]
    public class PluginSetting<T> : IPluginSetting, INotifyPropertyChanged where T : IEquatable<T>
    {
        public PluginSetting(string name, T initialValue)
        {
            this.Name = name;
            this.Value = initialValue;

            if (typeof(T) == typeof(double))
                SettingType = PluginSettingType.Double;
            else if (typeof(T) == typeof(bool))
                SettingType = PluginSettingType.Boolean;
        }
        
        public string Name { get; }

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                Notify();    
            }
        }

        private void Notify()
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs("Value"));
        }

        object IPluginSetting.Value { get => Value; set => Value = (T)value; }
        public PluginSettingType SettingType { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
