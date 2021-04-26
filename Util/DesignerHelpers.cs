using MyLights.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Util
{
    public static class DesignerHelpers
    {
        private static Random rand = new();
        private static Dictionary<string, int> counter = new();

        private static int Bump(string counterKey)
        {
            if (!counter.ContainsKey(counterKey))
                counter[counterKey] = 0;

            return counter[counterKey] += 1;
        }

        public static LightState GetLightState(bool white, bool random, double xr = 0.5)
        {
            Bump("light state");

            if (random)
                xr = rand.NextDouble();

            // in this case, xr represents the colortemp scale
            if (white)
            {
                return new LightState()
                {
                    Power = true,
                    Mode = LightMode.White,
                    Brightness = 0.9,
                    ColorTemp = xr * 1000.0,
                };
            }
            // for color mode, xr is hue
            else
            {
                return new LightState()
                {
                    Power = true,
                    Mode = LightMode.Color,
                    Color = new HSV(xr, 0.9, 0.9),
                };
            }
        }

        public static Scene GetScene(int nLights, bool white, bool random)
        {
            int n = Bump("scene");
            var scene = new Scene($"Test Scene #{n}");

            for (int i = 0; i < nLights; i++)
            {
                var state = GetLightState(white, random);
                scene.Add($"scene {counter["light state"]}", state);
            }

            return scene;
        }
    }
}
