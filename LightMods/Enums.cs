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
    public enum AttachmentsPoints
    {
        Default = 0b0,
        AllLights = 0b1,
        SingleLight = 0b10,
    }
}
