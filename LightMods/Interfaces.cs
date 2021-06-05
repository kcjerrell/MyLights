using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyLights.LightMods
{
    public interface ILightEffect
    {
        public void Attach(IModHost modHost, IList<LightViewModel> lights);
        public void Start();
        public void Suspend();
        public void Shutdown();
        public bool IsActive { get; }
        public IEnumerable<PluginSetting> Settings { get; }

        public event IsActiveChangedEventHandler IsActiveChanged;
    }

    public interface IModHost
    {
        public ReadOnlyObservableCollection<LightViewModel> LightViewModels { get; }
    }

    public delegate void IsActiveChangedEventHandler(ILightEffect sender, IsActiveChangedEventArgs e);

    public class IsActiveChangedEventArgs : EventArgs
    {
        public IsActiveChangedEventArgs(bool isActive)
        {
            this.IsActive = IsActive;
        }

        public bool IsActive { get; set; }

        public static IsActiveChangedEventArgs GetStatic(bool isActive)
        {
            if (isActive)
                return Active;
            else
                return Inactive;
        }

        private static IsActiveChangedEventArgs _active = new IsActiveChangedEventArgs(true);
        public static IsActiveChangedEventArgs Active { get => _active; }
        private static IsActiveChangedEventArgs _inactive = new IsActiveChangedEventArgs(false);
        public static IsActiveChangedEventArgs Inactive { get => _inactive; }
    }
}
