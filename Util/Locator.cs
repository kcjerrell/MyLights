using MyLights.Bridges;
using MyLights.Bridges.Udp;
using MyLights.Util;
using MyLights.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
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

        internal static Task StartServices()
        {
            var lb = lightBridge.ConnectAsync();
            var lib = Application.Current.Dispatcher.InvokeAsync(() => libraryVm.LoadLibrary(IsInDesignMode));
            return Task.WhenAll(lb, lib.Task);
        }

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
                designLightVm = lightBridge.LightVMs[0];
            }
            else
            {
                lightBridge = new UdpLightBridge();
            }

            if (IsInDesignMode)
            {
                libraryVm.LoadLibrary(true);
            }
        }
    }
}