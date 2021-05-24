using MyLights.Models;
using MyLights.Util;
using MyLights.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OCL = System.Collections.ObjectModel.ObservableCollection<MyLights.Models.Light>;
using OCLVM = System.Collections.ObjectModel.ObservableCollection<MyLights.ViewModels.LightViewModel>;

namespace MyLights.Bridges.Udp
{
    public partial class UdpLightBridge : ILightBridge
    {
        public UdpLightBridge()
        {

        }

        private const string BulbsRequest = "*bulb-.*";
            //"bulb-1"; // 

        public OCL Lights => lights;

        public OCLVM LightVMs => lightVms;

        public bool TryFindBulb(BulbRef key, out Light light)
        {
            var match = (from l in Lights
                         where key.Id == l.Id
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

        public async void Reload()
        {
            await udpClient.SendMessage(new LightDgram(DgramVerbs.Reload, "bridge"));
        }


        #region Static

        private static Logger log = new Logger("UdpLightBridge");

        private static Client udpClient;
        private static OCL lights = new OCL();
        private static OCLVM lightVms = new OCLVM();

        private static Dictionary<string, UdpPropertiesProvider> resourceIds = new Dictionary<string, UdpPropertiesProvider>();

        static async Task StartClient()
        {
            log.Log("Starting client...", 4);
            udpClient = new Client("127.0.0.1", 8090, 11000);
            await udpClient.Connect();

            udpClient.LightMessageReceived += UdpClient_LightMessageReceived;
        }

        private static async void UdpClient_LightMessageReceived(object sender, LightMessageEventArgs e)
        {
            await RecieveMessage(e.Message);
        }

        private static async Task RecieveMessage(LightDgram msg)
        {
            if (resourceIds.ContainsKey(msg.Target))
            {
                var props = resourceIds[msg.Target];
                props.ReceiveMessage(msg);
            }
            else
            {
                if (msg.Property == LightProperties.Id)
                {
                    log.Log("creating props", 4);
                    var props = new UdpPropertiesProvider();
                    props.PropertiesInitialized += Props_PropertiesInitialized;
                    await props.Enloop(msg, udpClient);
                    resourceIds.Add(msg.Target, props);
                }

                else
                {
                    log.Log("unknown target referenced, requesting more info");
                    var req = new LightDgram(DgramVerbs.Wonder, msg.Target, LightProperties.Id);
                    await udpClient.SendMessage(req);
                }
            }
        }

        private static void Props_PropertiesInitialized(object sender, System.EventArgs e)
        {
            log.Log("props are initialized...", 4);
                   
            if (sender is UdpPropertiesProvider props)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    log.Log("...creating light and vm", 4);
                    var light = new Light(props);
                    var lvm = new LightViewModel(light);

                    lights.Add(light);
                    lightVms.Add(lvm);
                });
            }
        }

        public static async Task GetLights(bool delay = false)
        {
            if (delay)
                await Task.Delay(2000);

            log.Log("requesting lights");
            var msg = new LightDgram(DgramVerbs.Wonder, BulbsRequest, LightProperties.Id);
            await udpClient.SendMessage(msg);
        }

        private async static Task StartServer()
        {
            if (Locator.IsInDesignMode)
                return;

            var procs = ProcessCommandLine.SearchProcCommandLine("node", "sockets");
            if (procs.Count == 0)
            {
                var startInfo = new ProcessStartInfo("node");
                startInfo.ArgumentList.Add("--trace-warnings");
                startInfo.ArgumentList.Add("--unhandled-rejections=warn");
                startInfo.ArgumentList.Add("--trace-uncaught");
                startInfo.ArgumentList.Add(@"C:\Users\kcjer\source\repos\lightrest\dist\sockets.js");
                Process.Start(startInfo);
                await Task.Delay(1000);
            }
        }

        public async Task ConnectAsync()
        {
            await StartServer();
            await StartClient();
            await GetLights(true);
        }

        #endregion
    }
}
