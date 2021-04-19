using MyLights.Models;
using MyLights.Util;
using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        public UdpLightBridge()
        {

        }

        public OCL Lights => UdpLightBridge.lights;

        public OCLVM LightVMs => UdpLightBridge.lightVms;

        public bool TryFindBulb(BulbRef key, out Light light)
        {
            var match = (from l in Lights
                         where key.Name == l.Name
                         select l).ToList();

            if (match.Count == 1)
            {
                light = match.Single();
                return true;
            }
            else
            {
                light = null;
                return false;
            }
        }

        #region Static

        private static Logger log = new Logger("UdpLightBridge");

        private static Client udpClient;
        private static OCL lights = new OCL();
        private static OCLVM lightVms = new OCLVM();
        private static bool isInDesignMode;

        private static Dictionary<string, UdpPropertiesProvider> resourceIds = new Dictionary<string, UdpPropertiesProvider>();

        static UdpLightBridge()
        {
            isInDesignMode = Locator.IsInDesignMode;

            if (isInDesignMode)
            {
                List<JsonBulb> jbulbs = new List<JsonBulb>();

                jbulbs.Add(new JsonBulb()
                {
                    Index = 0,
                    Name = "DesignBulb1",
                    Color = new HSV() { H = 0.2, S = 0.8, V = 1 },
                    Power = true,
                    Mode = "white"
                });
                jbulbs.Add(new JsonBulb()
                {
                    Index = 1,
                    Name = "DesignBulb2",
                    Color = new HSV() { H = 0.5, S = 0.6, V = 1 },
                    Power = true,
                    Mode = "color"
                });
            }
            else
            {
                StartClient().ContinueWith((t) => GetLights(true));
            }
        }

        static async Task StartClient()
        {
            //log.Log("Starting client...", 4);
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
                    //log.Log("creating props", 4);
                    var props = new UdpPropertiesProvider(msg.Target, msg.Data, udpClient, OnPropertiesInitialized);
                    resourceIds.Add(msg.Target, props);
                }

                else
                {
                    //log.Log("unknown target referenced, requesting more info");
                    var req = new LightDgram(DgramVerbs.Wonder, msg.Target, LightProperties.Name);
                    udpClient.SendMessage(req);
                }
            }
        }

        private static void OnPropertiesInitialized(UdpPropertiesProvider props)
        {
            //log.Log("props are initialized...", 4);

            App.Current.Dispatcher.Invoke(() =>
            {
                //log.Log("...creating light and vm", 4);
                var light = new Light(props);
                var lvm = new LightViewModel(light);

                lights.Add(light);
                lightVms.Add(lvm);
            });
        }

        public static async Task GetLights(bool delay = false)
        {
            if (delay)
                await Task.Delay(2000);

            //log.Log("requesting lights");
            var msg = new LightDgram(DgramVerbs.Wonder, "*bulb-.*", LightProperties.Name);
            await udpClient.SendMessage(msg);
        }
        #endregion
    }
}
