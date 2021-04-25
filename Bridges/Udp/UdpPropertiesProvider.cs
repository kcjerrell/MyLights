using MyLights.Models;
using MyLights.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static MyLights.Bridges.Udp.UdpLightBridge;

namespace MyLights.Bridges.Udp
{
    public class UdpPropertiesProvider : ILightPropertiesProvider
    {
        public UdpPropertiesProvider()
        {
            propertiesInitialized[DgramProperties.Power] = false;
            power = new UdpPower();
            power.OutgoingChangeRequested += OnOutgoingChangeRequested;

            propertiesInitialized[DgramProperties.Color] = false;
            color = new UdpColor();
            color.OutgoingChangeRequested += OnOutgoingChangeRequested;

            propertiesInitialized[DgramProperties.Mode] = false;
            mode = new UdpMode();
            mode.OutgoingChangeRequested += OnOutgoingChangeRequested;

            propertiesInitialized[DgramProperties.Brightness] = false;
            brightness = new UdpBrightness();
            brightness.OutgoingChangeRequested += OnOutgoingChangeRequested;

            propertiesInitialized[DgramProperties.ColorTemp] = false;
            colorTemp = new UdpColorTemp();
            colorTemp.OutgoingChangeRequested += OnOutgoingChangeRequested;

            properties = new UdpProperty[] { power, color, mode, brightness, colorTemp };

            //log = new Logger($"{resId} props");
            //log.Log("props started");
        }

        private void OnOutgoingChangeRequested(object sender, EventArgs e)
        {
            if (outgoing == null || outgoing.IsFaulted || outgoing.IsFaulted || outgoing.IsCanceled)
            {
                outgoing = ProcessOutgoing();
            }

        }

        private async Task ProcessOutgoing()
        {
            // This small delay (hopefully) will allow multiple property changes to be collected and sent together
            // For instance when a scene is loaded
            await Task.Delay(20);

            List<string> wishes = new(5);

            do
            {
                wishes.Clear();

                foreach (var p in properties)
                {
                    if (p.HasPendingChange)
                    {
                        wishes.Add(p.GetOutgoingFragment(true));
                    }
                }

                if (wishes.Count > 0)
                {
                    await client.SendMessage(LightDgram.MakeWish(ResourceId, wishes));

                    await Task.Delay(100);
                }
                else
                {
                    break;
                }

            }
            while (true);

            outgoing = null;

        }

        private Logger log;

        private Client client;

        private UdpPower power;
        private UdpColor color;
        private UdpMode mode;
        private UdpBrightness brightness;
        private UdpColorTemp colorTemp;
        private UdpProperty[] properties;

        private Task outgoing;

        Dictionary<DgramProperties, bool> propertiesInitialized = new Dictionary<DgramProperties, bool>();
        bool initialized = false;

        public int Index { get; private set; }

        public string Name { get; private set; }

        public IDeviceProperty<bool> PowerProperty => power;

        public IDeviceProperty<HSV> ColorProperty => color;

        public IDeviceProperty<string> ModeProperty => mode;

        public IDeviceProperty<double> BrightnessProperty => brightness;

        public IDeviceProperty<double> ColorTempProperty => colorTemp;

        public string ResourceId { get; private set; }

        public async Task Enloop(LightDgram msg, Client client)
        {
            ResourceId = msg.Target;
            Index = int.Parse(ResourceId.Split("-")[1]);

            Name = msg.Data;

            this.client = client;

            await client.SendMessage(new LightDgram(DgramVerbs.Enloop, ResourceId));
        }

        internal void ReceiveMessage(LightDgram msg)
        {
            if (Thread.CurrentThread.ManagedThreadId != App.Current.Dispatcher.Thread.ManagedThreadId)
            {
                App.Current.Dispatcher.Invoke(() => ReceiveMessage(msg));
            }
            else
            {
                switch (msg.Property)
                {
                    case DgramProperties.Power:
                        power.UpdateValue(bool.Parse(msg.Data));
                        break;
                    case DgramProperties.Mode:
                        mode.UpdateValue(msg.Data);
                        break;
                    case DgramProperties.Brightness:
                        brightness.UpdateValue(double.Parse(msg.Data));
                        break;
                    case DgramProperties.ColorTemp:
                        colorTemp.UpdateValue(double.Parse(msg.Data));
                        break;
                    case DgramProperties.Color:
                        color.UpdateValue(UdpColor.DecodeColor(msg.Data));
                        break;
                }

                if (!initialized)
                {
                    propertiesInitialized[msg.Property] = true;

                    if (propertiesInitialized.Values.All(p => p))
                    {
                        //Logger.Log("all props have value, calling initCallback");
                        initialized = true;
                        var handler = PropertiesInitialized;
                        handler?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        public event EventHandler PropertiesInitialized;
    }
}