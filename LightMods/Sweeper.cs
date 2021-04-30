using MyLights.Models;
using MyLights.Util;
using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyLights.LightMods
{
    public class SweeperEffect : IDeviceEffect
    {
        private LightViewModel lightViewModel;

        public SweeperEffect(LightViewModel lightViewModel)
        {
            this.lightViewModel = lightViewModel;
        }

        public async Task Sweep()
        {
            while (IsActive)
            {
                var col = lightViewModel.Color;

                double h = (col.H + 0.008) % 1.0;
                col = new Models.HSV(h, col.S, col.V);

                lightViewModel.Light.SetColor(col, true);

                await Task.Delay(100);
            }
        }

        public async void Start()
        {
            IsActive = true;
            Sweep();
        }

        public void Suspend()
        {
            IsActive = false;
        }

        public void Shutdown()
        {

        }

        public bool IsActive { get; private set; }
        public IEnumerable<IPluginSetting> Settings { get; }
        public ILightPlugin AssociatedPlugin { get; }
        public PluginProperties Properties { get; } = PluginProperties.DeviceEffect;
    }

    public class SweeperGlobal : IGlobalMod
    {
        private IModHost host;

        public SweeperGlobal(IModHost host)
        {
            this.host = host;
        }

        public async Task SweepLoop()
        {
            var lights = (from lvm in host.LightViewModels
                          select lvm.Light).ToList();
            int nLights = lights.Count;

            int p = 0;
            double lightOffset = 1.0 / nLights;

            while (IsActive)
            {
                for (int i = 0; i < nLights; i++)
                {
                    double h = (p / 400.0 + (double)i / nLights) % 1.0;
                    HSV col = lights[i].Color;
                    lights[i].SetColor(new HSV(h, col.S, col.V));
                }

                p = (p + 1) % 400;
                await Task.Delay(200);
            }
        }

        public void Start()
        {
            IsActive = true;
            SweepLoop();
        }

        public void Suspend()
        {
            IsActive = false;
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        public PluginProperties Properties { get; } = PluginProperties.GlobalMod;

        public bool IsActive { get; private set; }
        public ILightPlugin AssociatedPlugin { get; }
        public IEnumerable<IPluginSetting> Parameters { get; }
    }

    public class SweeperPlugin : ILightPlugin
    {
        public string Name { get; } = "Sweeper";
        public ImageSource Icon { get; } = new BitmapImage(new Uri("/Resource/Puzzles-256.png", UriKind.Relative));


        public IGlobalMod GetGlobalMod(IModHost host)
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
