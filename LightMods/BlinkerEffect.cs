using MyLights.Models;
using MyLights.Util;
using MyLights.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MyLights.LightMods
{
    public class BlinkerEffect : IDeviceEffect
    {
        public BlinkerEffect(LightViewModel lightViewModel, BlinkerPlugin plugin)
        {
            this.lightViewModel = lightViewModel;

            List<IPluginSetting> settings = new();
            settings.Add(hardBlinkSetting);
            settings.Add(delaySetting);
     
            delaySetting.PropertyChanged += DelaySetting_PropertyChanged;

            this.Settings = settings;
            this.AssociatedPlugin = plugin;
        }

        LightViewModel lightViewModel;
        private PluginSetting<bool> hardBlinkSetting = new("Hard Blink", false);
        // the double setting range will give 0-1, I'll map that to... 50-2000
        private PluginSetting<double> delaySetting = new("Delay", .2);
        int delayInterval = 400;

        private async Task Blink()
        {
            LightState state = new LightState(lightViewModel.Light);
            double altBright = state.Brightness < 505 ? 1000 : 10;
            HSV altColor = new HSV(state.Color.H, state.Color.S, state.Color.V < 0.5 ? 1.0 : 0.01);
            bool altBeat = false;

            while (IsActive)
            {
                if (hardBlinkSetting.Value)
                {
                    lightViewModel.Light.SetPower(altBeat, true);
                }
                else if (state.Mode == LightMode.Color)
                {
                    lightViewModel.Light.SetColor(altBeat ? altColor : state.Color, true);
                }
                else if (state.Mode == LightMode.White)
                {
                    lightViewModel.Light.SetBrightness(altBeat ? altBright : state.Brightness, true);
                }

                await Task.Delay(delayInterval);

                altBeat ^= true;
            }
        }

        private volatile bool _isActive;
        public bool IsActive { get => _isActive; private set => _isActive = value; }

        public void Start()
        {
            IsActive = true;
            Blink();
        }

        public void Suspend()
        {
            IsActive = false;
        }

        public void Shutdown()
        {
            
        }

        public ILightPlugin AssociatedPlugin { get; }
        public IEnumerable<IPluginSetting> Settings { get; }
        public PluginProperties Properties { get; } = PluginProperties.CanSuspend | PluginProperties.DeviceEffect;

        private void DelaySetting_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            delayInterval = (int)delaySetting.Value.MapRange(0.0, 1.0, 50, 2000);
        }
    }
}
