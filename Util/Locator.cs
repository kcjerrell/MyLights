using MyLights.Util;
using MyLights.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace MyLights.Util
{
    public class Locator : DependencyObject
    {
        public Locator()
        {
            if (Singleton != null)
                throw new System.Exception("Only make one!");

            else
                Singleton = this;

            IsInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());

            LightBridge = new LightBridge(IsInDesignMode);
        }

        internal LightBridge LightBridge { get; }

        internal ObservableCollection<LightViewModel> LightVMs { get => LightBridge.LightVMs; }

        public bool IsInDesignMode { get; }

        public static Locator Singleton { get; private set; }
    }
}