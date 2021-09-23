using MyLights.Bridges;
using MyLights.LightMods;
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
        public Random Rand { get => rand; }
        public ModHost ModHost => modHost;

        public static Locator Get { get; } = new Locator();

        internal static Task StartServices()
        {
            System.Diagnostics.Trace.WriteLine("does it work in design mode");
            var mod = modHost.Load(IsInDesignMode);
            var lb = lightBridge.ConnectAsync();
            var lib = Application.Current.Dispatcher.InvokeAsync(() => libraryVm.LoadLibrary(IsInDesignMode));
            return Task.WhenAll(lb, lib.Task, mod);
        }

        public static bool IsInDesignMode { get; }
        private static ILightBridge lightBridge;
        private static LightViewModel designLightVm;
        private static LibraryViewModel libraryVm = new LibraryViewModel();
        private static ModHost modHost;
        private static Random rand = new Random();

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
                //lightBridge = new UdpLightBridge();
                lightBridge = new Bridges.Udp2.Bridge();
            }

            if (IsInDesignMode)
            {
                libraryVm.LoadLibrary(true);
            }

            modHost = new ModHost(IsInDesignMode);
        }


        private Design _designVMs;
        public Design DesignVMs
        {
            get
            {
                if (_designVMs == null) { _designVMs = new Design(); }
                return _designVMs;
            }
        }


        public class Design
        {
            private MyLights.Models.SceneStop _sceneStop;
            public MyLights.Models.SceneStop DesignSceneStop
            {
                get
                {
                    if (_sceneStop == null) { _sceneStop = new(new(1, 1, 1), 50, Models.SceneTransition.Breath); }
                    return _sceneStop;
                }
            }

        }
    }
}