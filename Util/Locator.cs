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

            Library = new LibraryViewModel();
        }

        internal LightBridge LightBridge { get; }

        public ObservableCollection<LightViewModel> LightVMs { get => LightBridge.LightVMs; }

        public bool IsInDesignMode { get; }

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

    }
}