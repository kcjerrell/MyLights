using Flurl.Http;
using MyLights.Models;
using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OLightVMs = System.Collections.ObjectModel.ObservableCollection<MyLights.ViewModels.LightViewModel>;
using ROOLightVMs = System.Collections.ObjectModel.ReadOnlyObservableCollection<MyLights.ViewModels.LightViewModel>;

namespace MyLights.Util
{
    public class LightBridge
    {
        public LightBridge()
        {
            GetLightsCommand = new RelayCommand(async (p) => await GetLights());
        }

        public RelayCommand GetLightsCommand { get; set; }
        public IReadOnlyList<Light> Lights { get; } = lights;
        public ROOLightVMs LightVMs { get; } = new ROOLightVMs(lightVMs);

        public LightViewModel GetLightViewModel(Light light)
        {
            return (from lvm in LightVMs
                    where lvm.Light == light
                    select lvm).Single();
        }

        internal bool TryFindBulb(BulbRef key, out Light light)
        {
            var match = (from l in Lights
                         where l.Name == key.Name
                         select l).ToArray();

            if (match.Length == 1)
            {
                light = match[0];
                return true;
            }

            else
            {
                light = null;
                return false;
            }
        }




        public static LightBridge Singleton { get; private set; }

        static LightBridge()
        {
            Singleton = new LightBridge();
            isInDesignMode = Locator.IsInDesignMode;

            GetLights();
        }

        private static bool isInDesignMode;
        private static List<Light> lights = new List<Light>();
        private static OLightVMs lightVMs = new OLightVMs();

        
        public static async Task GetLights()
        {
            List<JsonBulb> jbulbs = new List<JsonBulb>();

            if (isInDesignMode)
            {
                jbulbs.Add(new JsonBulb()
                {
                    index = 0,
                    name = "DesignBulb1",
                    color = new HSV() { H = 0.2, S = 0.8, V = 1 },
                    power = true,
                    mode = "white"
                });
                jbulbs.Add(new JsonBulb()
                {
                    index = 1,
                    name = "DesignBulb2",
                    color = new HSV() { H = 0.5, S = 0.6, V = 1 },
                    power = true,
                    mode = "color"
                });
            }

            else
            {
                string url = @"http://localhost:1337/bulbs";
                try
                {
                    var root = await url.GetJsonAsync<JsonBulbRoot>();
                    jbulbs = root.bulbs;
                }
                catch (FlurlHttpException ex)
                {
                    if (!isInDesignMode)
                        throw;
                }
            }

            lightVMs.Clear();
            lights.Clear();

            foreach (var jBulb in jbulbs)
            {
                var light = Light.FromJson(jBulb);

                lightVMs.Add(new LightViewModel(light));
                lights.Add(light);
            }
        }
    }
}
