using MyLights.LightMods;
using MyLights.Util;
using MyLights.ViewModels;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyLights.LightMods
{
    public class BlinkerPlugin : ILightPlugin
    {
        public string Name => "Blinker";

        public ImageSource Icon { get; } = new BitmapImage(new Uri("/Puzzles-256.png", UriKind.Relative));

        public IDeviceEffect GetDeviceMod(LightViewModel lightViewModel)
        {
            return new BlinkerEffect(lightViewModel, this);
        }

        public PluginProperties Properties { get; } = PluginProperties.DeviceEffect | PluginProperties.GlobalMod;

        public IDeviceEffect GetGlobalMod(IModHost host)
        {
            return new BlinkerGlobal(host);
        }
    }
}
