using MyLights.Util;
using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyLights.LightMods
{
    /// <summary>
    /// Represents a class that is created per device
    /// </summary>
    public interface IDeviceEffect
    {
        public void Start();
        public void Suspend();
        public void Shutdown();
        public bool IsActive { get; }
        public IEnumerable<IPluginSetting> Settings { get; }
        public ILightPlugin AssociatedPlugin { get; }
        public PluginProperties Properties { get; }
    }

    public interface IPluginSetting
    {
        public string Name { get; }
        public object Value { get; set; }
        public PluginSettingType SettingType { get; }

    }

    public interface ILightPlugin
    {
        public string Name { get; }
        public ImageSource Icon { get; }
        public IDeviceEffect GetGlobalMod(IModHost host);
        public IDeviceEffect GetDeviceMod(LightViewModel lightViewModel);
        public PluginProperties Properties { get; }
    }
}
