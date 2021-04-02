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
            LightBridge = LightBridge.Singleton;
            Library = new LibraryViewModel();
        }

        public LightBridge LightBridge { get; }
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

        public LightViewModel DesignLightVM { get => LightVMs[0]; }
        public static bool IsInDesignMode { get; }
        static Locator()
        {
            IsInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
        }
    }
}