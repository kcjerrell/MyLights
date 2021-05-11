using MyLights.Models;
using MyLights.Util;
using MyLights.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyLights.LightMods
{
    [SingleLightEffect("Blinker", "/Resource/Puzzles-256.png")]
    public class BlinkerEffect : ILightEffect
    {
        IList<LightViewModel> lights;
        CancellationTokenSource cancelSource;
        Task blinkTask;

        bool hardBlinkSetting = false;
        int delayInterval = 350;

        public async Task Blink(CancellationToken token)
        {
            var light = lights[0];
            LightState state = new LightState(light.Light);
            double altBright = state.Brightness < 505 ? 1000 : 10;
            HSV altColor = new HSV(state.Color.H, state.Color.S, state.Color.V < 0.5 ? 1.0 : 0.01);
            bool altBeat = false;

            while (IsActive)
            {
                //foreach (var light in lights)
                //{
                    if (hardBlinkSetting)
                    {
                        light.Light.SetPower(altBeat, true);
                    }
                    else if (state.Mode == LightMode.Color)
                    {
                        light.Light.SetColor(altBeat ? altColor : state.Color, true);
                    }
                    else if (state.Mode == LightMode.White)
                    {
                        light.Light.SetBrightness(altBeat ? altBright : state.Brightness, true);
                    }
                //}

                await Task.Delay(delayInterval);

                altBeat ^= true;

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

        public void Start()
        {
            IsActive = true;
            cancelSource = new CancellationTokenSource();
            blinkTask = Blink(cancelSource.Token).ContinueWith((task) =>
            {
                cancelSource.Dispose();
                cancelSource = null;
            });
        }

        public void Suspend()
        {
            cancelSource?.Cancel();
        }

        public void Shutdown()
        {
            cancelSource?.Cancel();
        }

        public bool IsActive { get; set; }
        public IEnumerable<PluginSetting> Settings { get; }
    }
}
