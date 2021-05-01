using MyLights.Util;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyLights.LightMods
{
    public class BlinkerGlobal : IDeviceEffect
    {
        public BlinkerGlobal(IModHost host)
        {
            this.modHost = host;
        }

        IModHost modHost;
        Task blinkTask;
        CancellationTokenSource cancelSource;

        public async Task Blink(CancellationToken cancelToken)
        {
            var lights = (from lvm in modHost.LightViewModels
                          select lvm.Light).ToList();
            int beat = 0;

            while (!cancelToken.IsCancellationRequested)
            {
                for (int i = 0; i < lights.Count; i++)
                {
                    if (i == beat)
                        lights[i].SetPower(true, true);
                    else
                        lights[i].SetPower(false, true);
                }

                beat += 1;
                beat %= lights.Count;

                await Task.Delay(200);
            }

            cancelSource.Dispose();
        }

        public void Start()
        {
            cancelSource = new CancellationTokenSource();
            blinkTask = Task.Run(() => Blink(cancelSource.Token), cancelSource.Token);            
            IsActive = true;
        }

        public void Suspend()
        {
            cancelSource.Cancel();
            IsActive = false;
        }

        public void Shutdown()
        {
            
        }

        public bool IsActive { get; private set; }
        public ILightPlugin AssociatedPlugin { get; }
        public IEnumerable<IPluginSetting> Settings { get; }
        public PluginProperties Properties { get; } = PluginProperties.GlobalMod | PluginProperties.CanSuspend;
    }
}
