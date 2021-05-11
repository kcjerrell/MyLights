using MyLights.Util;
using MyLights.ViewModels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyLights.LightMods
{
    [SingleLightEffect("Sweeper", "/Resource/Puzzles-256.png")]
    [MultiLightEffect("Sweeper", "/Resource/Puzzles-256.png")]
    public class SweeperEffect : ILightEffect
    {
        private IList<LightViewModel> lights;
        private CancellationTokenSource cancelSource;
        private Task sweepTask;

        public bool IsActive { get; private set; }
        public List<PluginSetting> Settings { get; }

        public async Task Sweep(CancellationToken token)
        {
            while (IsActive)
            {
                for (int i = 0; i < lights.Count; i++)
                {
                    var light = lights[i].Light;
                    var col = light.Color;
                    double h = (col.H + 0.008) % 1.0;
                    col = new Models.HSV(h, col.S, col.V);
                    light.SetColor(col, true);
                }

                await Task.Delay(200);

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
            sweepTask = Sweep(cancelSource.Token).ContinueWith((task) =>
            {
                cancelSource.Dispose();
                cancelSource = null;
            });
        }
    }
}
