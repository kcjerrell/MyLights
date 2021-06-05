using MyLights.Models;
using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyLights.LightMods
{
    [SingleLightEffect("Bubble", "MyLights.Resources.Puzzles-256.png")]
    [MultiLightEffect("Bubble", "MyLights.Resources.Puzzles-256.png")]
    public class BubblerEffect : ILightEffect
    {
        private IList<LightViewModel> lights;
        private CancellationTokenSource cancelSource;
        private Task sweepTask;
        private bool isActive;

        public bool IsActive
        {
            get => isActive;
            private set
            {
                isActive = value;
                var handler = IsActiveChanged;
                handler?.Invoke(this, new IsActiveChangedEventArgs(IsActive));
            }
        }
        public List<PluginSetting> Settings { get; }

        const double peakV = 0.9;
        const double baseV = 0.2;
        private static TimeSpan totalFadeTime = TimeSpan.FromSeconds(1.5);
        const int delayInterval = 50;

        public async Task Bubble(CancellationToken token)
        {
            foreach (var light in lights)
            {
                var col = light.Color;
                col.V = peakV;

                light.Light.SetColor(col, true);
            }

            await Task.Delay(totalFadeTime);

            foreach (var light in lights)
            {
                var col = light.Color;
                col.V = baseV;

                light.Light.SetColor(col, true);
            }

            IsActive = false;

            // var startTime = DateTime.Now;
            // 
            // foreach (var light in lights)
            // {
            //     light.Light.SetMode(Models.LightMode.Color);
            //     light.V = baseV;
            // }
            // 
            // double a = peakV;
            // double b = baseV;
            // double ab = b - a;                    
            // 
            // while (IsActive)
            // {
            //     var elapsed = DateTime.Now - startTime;
            //     double tr = Math.Min(elapsed / totalFadeTime, 1.0);
            //     double v = a + tr * ab;
            // 
            // 
            //     foreach(var light in lights)
            //     {
            //         var col = new HSV(light.Color.H, light.Color.S, v);
            //         light.Light.SetColor(col, true);
            //     }                
            // 
            //     if (tr == 1.0)
            //     {
            //         IsActive = false;
            //         return;
            //     }
            // 
            //     await Task.Delay(delayInterval);
            // 
            //     if (token.IsCancellationRequested)
            //     {
            //         IsActive = false;
            //     }
            // }
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

        public event IsActiveChangedEventHandler IsActiveChanged;
    }
}
