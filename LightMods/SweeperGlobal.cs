using MyLights.Models;
using MyLights.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLights.LightMods
{
    public class SweeperGlobal : IDeviceEffect
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
                    double h = (p / 360.0 + (double)i / nLights) % 1.0;
                    HSV col = lights[i].Color;
                    lights[i].SetColor(new HSV(h, col.S, col.V), true);
                }

                p = (p + 1) % 360;
                await Task.Delay(100);
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
        public IEnumerable<IPluginSetting> Settings { get; }
    }
}
