using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyLights.LightMods
{
    [MultiLightEffect("Bubbler", "/Resource/Puzzles-256.png")]
    public class BubblerEffect : ILightEffect
    {
        private IList<LightViewModel> lights;
        private CancellationTokenSource cancelSource;
        private Task sweepTask;

        public bool IsActive { get; private set; }
        public List<PluginSetting> Settings { get; }

        const double peakV = 0.9;
        const double baseV = 0.2;
        const double totalFadeTime = 1500.0;
        const int delayInterval = 200;

        public async Task Bubble(CancellationToken token)
        {
            foreach (var light in lights)
            {
                light.Light.SetMode(Models.LightMode.Color);
                light.V = baseV;
            }

            int li = 0;
            double decay = (peakV - baseV) / totalFadeTime / (delayInterval / 1000.0);
            int totalProc = (int)Math.Floor(totalFadeTime / delayInterval);
            int p = totalProc;
            double range = peakV - baseV;

            while (IsActive)
            {
                var light = lights[li].Light;
                var col = light.Color;
                double v = p * range + baseV;
                col = new Models.HSV(col.H, col.S, v);
                               
                light.SetColor(col, true);

                p -= 1;

                if (p < 0)
                {
                    p = totalProc;
                    li = (li + 1) % lights.Count;
                }

                await Task.Delay(delayInterval);

                if (token.IsCancellationRequested)
                {
                    IsActive = false;
                }
            }
        }

        public void Attach(IModHost modHost, IList<LightViewModel> lights)
        {
            this.lights = lights;
        }

        public void Suspend()
        {
            cancelSource?.Cancel();
        }

        public void Shutdown()
        {
            cancelSource?.Cancel();
        }

        IEnumerable<PluginSetting> ILightEffect.Settings { get; }

        public void Start()
        {
            IsActive = true;
            cancelSource = new CancellationTokenSource();
            sweepTask = Bubble(cancelSource.Token).ContinueWith((task) =>
            {
                cancelSource.Dispose();
                cancelSource = null;
            });
        }
    }
}
