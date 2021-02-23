using Flurl.Http;
using MyLights.Models;
using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Util
{
    internal class LightBridge
    {
        public LightBridge(bool isInDesignMode)
        {
            this.isInDesignMode = isInDesignMode;
            GetLights();
        }

        private bool isInDesignMode;

        private List<Light> lights = new List<Light>();
        public ObservableCollection<LightViewModel> LightVMs { get; private set; } = new ObservableCollection<LightViewModel>();

        private async void GetLights()
        {
            List<JsonBulb> jbulbs = new List<JsonBulb>();

            if (isInDesignMode)
            {
                jbulbs.Add(new JsonBulb()
                {
                    index = 0,
                    name = "DesignBulb1",
                    color = new JsonColor() { r = 200, g = 200, b = 100 },
                    power = true,
                });
                jbulbs.Add(new JsonBulb()
                {
                    index = 1,
                    name = "DesignBulb2",
                    color = new JsonColor() { r = 200, g = 100, b = 200 },
                    power = true,
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

            foreach (var jBulb in jbulbs)
            {
                var light = new Light(jBulb);

                LightVMs.Add(new LightViewModel(light));
                lights.Add(light);
            }
        }

    }
}
