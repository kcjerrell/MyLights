using MyLights.Util;
using MyLights.ViewModels;
using System;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyLights.LightMods
{
    public class SweeperPlugin : ILightPlugin
    {
        public string Name { get; } = "Sweeper";
        public ImageSource Icon { get; } = new BitmapImage(new Uri("/Resource/Puzzles-256.png", UriKind.Relative));


        public IDeviceEffect GetGlobalMod(IModHost host)
        {
            return new SweeperGlobal(host);
        }

        public IDeviceEffect GetDeviceMod(LightViewModel lightViewModel)
        {
            return new SweeperEffect(lightViewModel);
        }

        public PluginProperties Properties { get; } = PluginProperties.DeviceEffect | PluginProperties.GlobalMod;
    }
}
