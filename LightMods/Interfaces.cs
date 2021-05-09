using MyLights.ViewModels;
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
    }

    public interface IModHost
    {
        public ReadOnlyObservableCollection<LightViewModel> LightViewModels { get; }
    }
}
