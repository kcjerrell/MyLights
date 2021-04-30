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

    public enum PluginSettingType
    {
        Default,
        Double,
        Boolean,
    }

    public interface ILightPlugin
    {
        public string Name { get; }
        public ImageSource Icon { get; }
        public IGlobalMod GetGlobalMod();
        public IDeviceEffect GetDeviceMod(LightViewModel lightViewModel);
        public PluginProperties Properties { get; }
    }

    public interface IGlobalMod
    {
        public void Start();

        public void Suspend();

        public void Shutdown();

        public bool IsActive { get; }

        public ILightPlugin AssociatedPlugin { get; }
        public IEnumerable<IPluginSetting> Parameters { get; }

    }

    [Flags]
    public enum PluginProperties
    {
        Default = 0b0,
        GlobalMod = 0b1,
        DeviceEffect = 0b10,
        CanSuspend = 0b100,

    }
}
