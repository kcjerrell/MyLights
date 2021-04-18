using MyLights.Models;
using MyLights.Util;
using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using OCL = System.Collections.ObjectModel.ObservableCollection<MyLights.Models.Light>;
using OCLVM = System.Collections.ObjectModel.ObservableCollection<MyLights.ViewModels.LightViewModel>;

namespace MyLights.LightUdp
{
    public class UdpLightBridge : ILightBridge
    {
        public OCL Lights => UdpLightBridge.lights;

        public OCLVM LightVMs => UdpLightBridge.lightVms;

        public bool TryFindBulb(BulbRef key, out Light light)
        {
            throw new Exception();
        }

        private static Client udpClient;
        private static OCL lights = new OCL();
        private static OCLVM lightVms = new OCLVM();
        private static bool isInDesignMode;

        private static Dictionary<string, UdpPropertiesProvider> resourceIds = new Dictionary<string, UdpPropertiesProvider>();

        static UdpLightBridge()
        {
            isInDesignMode = Locator.IsInDesignMode;

            StartClient().ContinueWith((t) => GetLights(true));
        }

        static async Task StartClient()
        {
            udpClient = new Client("127.0.0.1", 8090, 11000);
            await udpClient.Connect();

            udpClient.LightMessageReceived += UdpClient_LightMessageReceived;
        }

        private static void UdpClient_LightMessageReceived(object sender, LightMessageEventArgs e)
        {
            var msg = e.Message;

            if (resourceIds.ContainsKey(msg.Target))
            {
                var props = resourceIds[msg.Target];
                props.ReceiveMessage(msg);
            }
            else
            {
                if (msg.Property == LightProperties.Name)
                {
                    var props = new UdpPropertiesProvider(msg.Target, msg.Data, udpClient, OnPropertiesInitialized);
                    resourceIds.Add(msg.Target, props);
                }

                else
                {
                    var req = new LightDgram(DgramVerbs.Wonder, msg.Target, LightProperties.Name);
                    udpClient.SendMessage(req);
                }
            }
        }

        private static void OnPropertiesInitialized(UdpPropertiesProvider props)
        {
            App.Current.MainWindow.Dispatcher.Invoke(() =>
            {
                var light = new Light(props);
                var lvm = new LightViewModel(light);

                lights.Add(light);
                lightVms.Add(lvm);
                Console.WriteLine("hey");
            });
        }

        public static async Task GetLights(bool delay = false)
        {
            if (delay)
                await Task.Delay(2000);

            var msg = new LightDgram(DgramVerbs.Wonder, "*bulb-.*", LightProperties.Name);
            await udpClient.SendMessage(msg);
        }
    }
}
