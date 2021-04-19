using MyLights.Models;
using MyLights.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyLights.LightUdp
{
    public class UdpPropertiesProvider : ILightPropertiesProvider
    {
        public UdpPropertiesProvider(string resId, string name, Client client, Action<UdpPropertiesProvider> initCallback)
        {
            ResourceId = resId;
            Index = int.Parse(resId.Split("-")[1]);

            Name = name;

            this.client = client;

            propertiesInitialized[LightProperties.Power] = false;
            power = new UdpPower(client, resId);

            propertiesInitialized[LightProperties.Color] = false;
            color = new UdpColor(client, resId);

            propertiesInitialized[LightProperties.Mode] = false;
            mode = new UdpMode(client, resId);

            propertiesInitialized[LightProperties.Brightness] = false;
            brightness = new UdpBrightness(client, resId);

            propertiesInitialized[LightProperties.ColorTemp] = false;
            colorTemp = new UdpColorTemp(client, resId);

            this.initCallback = initCallback;

            client.SendMessage(new LightDgram(DgramVerbs.Enloop, ResourceId));

            log = new Logger($"{resId} props");
            //log.Log("props started");
        }

        private Logger log;

        private Client client;

        private UdpPower power;
        private UdpColor color;
        private UdpMode mode;
        private UdpBrightness brightness;
        private UdpColorTemp colorTemp;

        Dictionary<LightProperties, bool> propertiesInitialized = new Dictionary<LightProperties, bool>();
        bool initialized = false;
        Action<UdpPropertiesProvider> initCallback;

        public int Index { get; }

        public string Name { get; set; }

        public IDeviceProperty<bool> PowerProperty => power;

        public IDeviceProperty<HSV> ColorProperty => color;

        public IDeviceProperty<string> ModeProperty => mode;

        public IDeviceProperty<double> BrightnessProperty => brightness;

        public IDeviceProperty<double> ColorTempProperty => colorTemp;

        public string ResourceId { get; }

        internal void ReceiveMessage(LightDgram msg)
        {
            switch (msg.Property)
            {
                case LightProperties.Power:
                    power.UpdateValue(bool.Parse(msg.Data));
                    break;
                case LightProperties.Mode:
                    mode.UpdateValue(msg.Data);
                    break;
                case LightProperties.Brightness:
                    brightness.UpdateValue(double.Parse(msg.Data));
                    break;
                case LightProperties.ColorTemp:
                    colorTemp.UpdateValue(double.Parse(msg.Data));
                    break;
                case LightProperties.Color:
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
                    initCallback(this);
                }
            }
        }
    }
}