using MyLights.Models;
using MyLights.Util;
using MyLights.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace MyLights.Util
{
    public class Locator : DependencyObject, INotifyPropertyChanged
    {
        public Locator()
        {
            //LightBridge = new RestLightBridge();
            LightBridge = new LightUdp.UdpLightBridge();
            Library = new LibraryViewModel();

            if (IsInDesignMode)
            {
                LightVMs.CollectionChanged += (s, e) =>
                {
                    if (LightVMs.Count >= 1)
                        DesignLightVM = LightVMs[0];
                };
            }
        }

        public ILightBridge LightBridge { get; }
        public ObservableCollection<LightViewModel> LightVMs { get => LightBridge.LightVMs; }
        public LibraryViewModel Library { get; private set; }

        private MainWindowViewModel _mainWindowViewModel;
        public MainWindowViewModel MainWindowViewModel
        {
            get
            {
                if (_mainWindowViewModel == null) { _mainWindowViewModel = new MainWindowViewModel(); }
                return _mainWindowViewModel;
            }
        }

        SceneSetter _designSceneSetter;

        public event PropertyChangedEventHandler PropertyChanged;

        public SceneSetter DesignSceneSetter
        {
            get
            {
                if (_designSceneSetter == null)
                {
                    _designSceneSetter = new SceneSetter(new BulbRef() { Name = "Up" });
                    _designSceneSetter.Mode = "color";
                    _designSceneSetter.Color = new Models.HSV(1, 1, 1);
                }
                return _designSceneSetter;
            }
        }

        public LightViewModel DesignLightVM { get; private set; }
        public static bool IsInDesignMode { get; }
        static Locator()
        {
            IsInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
        }
    }
}