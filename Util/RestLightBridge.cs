using Flurl.Http;
using MyLights.Bridges;
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
    public class RestLightBridge : ILightBridge
    {
        public RestLightBridge()
        {
            GetLightsCommand = new RelayCommand(async (p) => await GetLights());
        }

        public RelayCommand GetLightsCommand { get; set; }
        public ObservableCollection<Light> Lights { get; } = lights;
        public OLightVMs LightVMs { get; } = lightVMs;

        public LightViewModel GetLightViewModel(Light light)
        {
            return (from lvm in LightVMs
                    where lvm.Light == light
                    select lvm).Single();
        }

        public bool TryFindBulb(BulbRef key, out Light light)
        {
            var match = (from l in Lights
                         where l.Id == key.Id
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

        public static RestLightBridge Singleton { get; private set; }

        static RestLightBridge()
        {
            Singleton = new RestLightBridge();
            isInDesignMode = Locator.IsInDesignMode;

            GetLights(true);
        }

        private static bool isInDesignMode;
        private static ObservableCollection<Light> lights = new ObservableCollection<Light>();
        private static OLightVMs lightVMs = new OLightVMs();

        
        public static async Task GetLights(bool delay = false)
        {
            if (delay)
                await Task.Delay(2000);

            if (!await LightREST.CheckServer())
                return;

            List<JsonBulb> jbulbs = new List<JsonBulb>();

            if (isInDesignMode)
            {
                jbulbs.Add(new JsonBulb()
                {
                    Index = 0,
                    Id = "DesignBulb1",
                    Color = new HSV() { H = 0.2, S = 0.8, V = 1 },
                    Power = true,
                    Mode = LightMode.White
                });
                jbulbs.Add(new JsonBulb()
                {
                    Index = 1,
                    Id = "DesignBulb2",
                    Color = new HSV() { H = 0.5, S = 0.6, V = 1 },
                    Power = true,
                    Mode = LightMode.Color
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
                var light = new Light(jBulb);

                lightVMs.Add(new LightViewModel(light));
                lights.Add(light);
            }
        }

        public Task ConnectAsync()
        {
            throw new NotImplementedException();
        }

        public void Reload()
        {
            throw new NotImplementedException();
        }
    }
}
