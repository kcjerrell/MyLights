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
            IsInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());

            LightBridge = new LightBridge(IsInDesignMode);
        }

        internal LightBridge LightBridge { get; }

        public ObservableCollection<LightViewModel> LightVMs { get => LightBridge.LightVMs; }

        public bool IsInDesignMode { get; }
    }
}