using MyLights.Models;
using MyLights.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyLights.Bridges.Node2
{
    public class Bridge : ILightBridge
    {
        public ObservableCollection<Light> Lights => lights;
        public ObservableCollection<LightViewModel> LightVMs => lightVms;

        public bool TryFindBulb(BulbRef key, out Light light)
        {
            light = (from l in lights
                     where l.Id == key.Id
                     select l).SingleOrDefault();

            return (light != null);
        }

        public async Task ConnectAsync()
        {
            await StartClient();
            await GetLights();
        }

        private static ObservableCollection<Light> lights = new();
        private static ObservableCollection<LightViewModel> lightVms = new();
        private static Dictionary<int, Provider> providers = new();
        private static NodeService nodeService;

        static async Task StartClient()
        {
            if (nodeService == null)
            {
                nodeService = new();
                await nodeService.Connect();

                nodeService.MessageReceived += nodeService_MessageReceived;
            }
        }

        private static void nodeService_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            // Trace.WriteLine(e.Message);

            // bulb/index/properties
            // or
            // bulb/index/properties/bulb/index/properties/bulb/index/properties

            // actually now they are just going to be one prop per message
            // bulb/2/power/true
            // bulb/5/name/deggo

            var split = e.Message.Split('/');
            string resourceType = split[0];
            int index = int.Parse(split[1]);
            string key = split[2];
            string value = split[3];


            if (providers.ContainsKey(index))
            {
                providers[index].Update(key, value);
            }
            else
            {
                var provider = new Provider(index, nodeService);
                providers.Add(index, provider);

                var light = new Light(provider);
                lights.Add(light);

                var lightVm = new LightViewModel(light);
                lightVms.Add(lightVm);

                providers[index].Update(key, value);
            }
        }

        static async Task GetLights()
        {
            await nodeService.SendMessage("get", "bulbs");
        }

        async public void Reload()
        {
            lights.Clear();
            lightVms.Clear();
            providers.Clear();
            await nodeService.SendMessage("reload");
        }
    }
}
