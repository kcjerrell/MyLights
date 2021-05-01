using System;

namespace MyLights.LightMods
{
    public enum PluginSettingType
    {
        Default,
        Double,
        Boolean,
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
