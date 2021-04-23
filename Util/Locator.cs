using MyLights.Models;
using MyLights.Util;
using MyLights.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace MyLights.Util
{
    public class Locator
    {
        public Locator()
        {

        }

        public ILightBridge LightBridge => lightBridge;
        public ObservableCollection<LightViewModel> LightVMs => lightBridge.LightVMs;
        public LibraryViewModel Library => libraryVm;
        public LightViewModel DesignLightVM { get; private set; }


        public static Locator Get { get; } = new Locator();
        public static bool IsInDesignMode { get; }
        private static ILightBridge lightBridge;
        private static LightViewModel designLightVm;
        private static LibraryViewModel libraryVm = new LibraryViewModel();
        static Locator()
        {
            IsInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
            
            if (IsInDesignMode)
            {
                lightBridge = new TestLightBridge();
            }
            else
            {
                lightBridge = new LightUdp.UdpLightBridge();
            }

            lightBridge.LightVMs.CollectionChanged += (s, e) =>
            {
                if (lightBridge.LightVMs.Count >= 1)
                    designLightVm = lightBridge.LightVMs[0];
            };
        }
    }
}