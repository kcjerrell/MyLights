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
    public class LightBridge
    {
        public LightBridge(bool isInDesignMode)
        {
            this.isInDesignMode = isInDesignMode;
            GetLightsCommand = new RelayCommand((p) => GetLights());

            //GetLights();
        }

        private bool isInDesignMode;

        private List<Light> lights = new List<Light>();

        internal bool TryFindBulb(BulbRef key, out Light light)
        {
            var match = (from l in lights
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

        public event EventHandler LightsUpdated;

        public ObservableCollection<LightViewModel> LightVMs { get; private set; } = new ObservableCollection<LightViewModel>();
        public RelayCommand GetLightsCommand { get; set; }

        public LightViewModel GetLightViewModel(Light light)
        {
            return (from lvm in LightVMs
                    where lvm.Light == light
                    select lvm).Single();
        }

        public async Task GetLights()
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

            LightVMs.Clear();
            lights.Clear();

            foreach (var jBulb in jbulbs)
            {
                var light = new Light(jBulb);

                LightVMs.Add(new LightViewModel(light));
                lights.Add(light);
            }

            var handler = LightsUpdated;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}
