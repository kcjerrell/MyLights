using MyLights.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

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
}
