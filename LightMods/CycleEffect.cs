using MyLights.Models;
using MyLights.Util;
using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyLights.LightMods
{
    [MultiLightEffect("Cycle", "none")]
    public class CycleEffect : LightEffectBase
    {
        public CycleEffect()
        {
            Settings.Add(procDelaySetting);
            procDelaySetting.PropertyChanged += ProcDelaySetting_PropertyChanged;
        }

        private void ProcDelaySetting_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            procDelay = (int)procDelaySetting.Value;
        }

        private double[] hues = new double[] { 0.33, 0.66, 0.49, 0.82, 0.28, 0.9};
        private const double hueVary = 0.05;

        private double sat = 0.95;
        private const double satVary = 0.05;

        private double val = 0.7;
        private const double valVary = 0.2;

        private int[] prog;

        private int procDelay = 2000;
        private const int procDelayVary = 1900;

        private NumericPluginSetting procDelaySetting = new NumericPluginSetting(2000, 50, 10000, "Proc Delay");

        protected override async Task ProcLoop(CancellationToken token)
        {
            prog = Enumerable.Repeat(0, Lights.Count).ToArray();
            int n = 0;

            while (!token.IsCancellationRequested)
            {
                //n = Locator.Get.Rand.Next(Lights.Count);
                //var light = Lights[n];
                //var color = new HSV(hues[prog[n]].PlusOrMinus(hueVary),
                //                              sat.PlusOrMinus(satVary),
                //                              val.PlusOrMinus(valVary));

                //light.Light.SetColor(color, true);

                //prog[n] = (prog[n] + 1) % hues.Length;
                //// n = n.IncWrap(Lights.Count);

                //await Task.Delay(procDelay);

                ////if (n == 0)
                ////    await Task.Delay(6000);
                ///

                foreach (var light in Lights)
                {
                    light.Light.SetPower(!light.Power, true);
                }

                await Task.Delay(500);
            }
        }
    }
}
