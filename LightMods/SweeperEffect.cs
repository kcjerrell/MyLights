using MyLights.Util;
using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyLights.LightMods
{
    [SingleLightEffect("Sweeper", "/Resource/Puzzles-256.png")]
    [MultiLightEffect("Sweeper", "/Resource/Puzzles-256.png")]
    public class SweeperEffect : ILightEffect
    {
        public SweeperEffect()
        {
            Settings = new();
            // 5 to 600 I guess (in seconds)
            periodSetting = new NumericPluginSetting(60, 5, 600, "Period");
            periodSetting.PropertyChanged += PeriodSetting_PropertyChanged;
            Settings.Add(periodSetting);
        }

        private void PeriodSetting_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.period = TimeSpan.FromSeconds(periodSetting.Value);
        }

        // I have not tested the limits of this system yet
        // but 20 updates a second is a good minimum to start with
        // maybe... that would 18 seconds for the whole spectrum
        // doesn't matter right now though
        const int minIntervalMs = 50;
        const int maxIntervalMs = 10000;

        private IList<LightViewModel> lights;
        private CancellationTokenSource cancelSource;
        private Task sweepTask;
        private NumericPluginSetting periodSetting;
        //private int interval = 200;
        private TimeSpan period = TimeSpan.FromSeconds(60);

        public bool IsActive { get; private set; }
        public List<PluginSetting> Settings { get; }

        public async Task Sweep(CancellationToken token)
        {
            // there are 360 discrete values across the spectrum using Tuya's (stupid?) format
            int interval = (int)(period / 360.0).TotalMilliseconds.Clamp(minIntervalMs, maxIntervalMs);
            double step = 1.0 / 360.0;

            while (IsActive)
            {
                for (int i = 0; i < lights.Count; i++)
                {
                    var light = lights[i].Light;
                    var col = light.Color;
                    double h = (col.H + step) % 1.0;
                    col = new Models.HSV(h, col.S, col.V);
                    light.SetColor(col, true);
                }

                await Task.Delay(interval);

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

        public event IsActiveChangedEventHandler IsActiveChanged;
    }
}
