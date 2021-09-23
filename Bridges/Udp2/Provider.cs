using MyLights.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Bridges.Udp2
{
    class Provider : ILightPropertiesProvider
    {
        public Provider(int index, Client udpClient)
        {
            Index = index;
            client = udpClient;

            power = new Udp2LightProperties.Power();
            power.ChangeRequested += OnChangeRequested;

            color = new Udp2LightProperties.Color();
            color.ChangeRequested += OnChangeRequested;

            mode = new Udp2LightProperties.Mode();
            mode.ChangeRequested += OnChangeRequested;

            brightness = new Udp2LightProperties.Brightness();
            brightness.ChangeRequested += OnChangeRequested;

            colorTemp = new Udp2LightProperties.ColorTemp();
            colorTemp.ChangeRequested += OnChangeRequested;

            scene = new Udp2LightProperties.Scene();
            scene.ChangeRequested += OnChangeRequested;

            properties = new LightProperty[] { power, color, mode, brightness, colorTemp, scene };
        }

        private void OnChangeRequested<T>(LightProperty<T> sender, bool immediate)
        {
            if (outgoing == null || outgoing.IsFaulted || outgoing.IsFaulted || outgoing.IsCanceled)
            {
                outgoing = ProcessOutgoing();
            }
        }

        private async Task ProcessOutgoing()
        {
            await Task.Delay(20);

            do
            {
                var props = (from p in properties
                             where p.HasPendingChange
                             select p.GetProperty(true)).ToArray();

                if (props.Length == 0)
                    break;

                foreach (var prop in props)
                {
                    await client.SendMessage("set", Index.ToString(), prop);
                }

                // await client.SendMessage("set", Index.ToString(), String.Join('&', props));
            }
            while (true);

            outgoing = null;
        }

        private Client client;
        private Task outgoing;

        private Udp2LightProperties.Power power;
        private Udp2LightProperties.Color color;
        private Udp2LightProperties.Mode mode;
        private Udp2LightProperties.Brightness brightness;
        private Udp2LightProperties.ColorTemp colorTemp;
        private Udp2LightProperties.Scene scene;
        private LightProperty[] properties;

        public int Index { get; private set; }
        public string Id { get; private set; }
        public IDeviceProperty<bool> PowerProperty => this.power;
        public IDeviceProperty<HSV> ColorProperty => this.color;
        public IDeviceProperty<LightMode> ModeProperty => this.mode;
        public IDeviceProperty<double> BrightnessProperty => this.brightness;
        public IDeviceProperty<double> ColorTempProperty => this.colorTemp;
        public IDeviceProperty<string> SceneProperty => this.scene;

        internal void Update(string key, string value)
        {            
            switch (key)
            {
                case "color":
                    color.UpdateValue(value);
                    break;
                case "mode":
                    mode.UpdateValue(value);
                    break;
                case "power":
                    power.UpdateValue(bool.Parse(value));
                    break;
                case "brightness":
                    brightness.UpdateValue(double.Parse(value));
                    break;
                case "colortemp":
                    colorTemp.UpdateValue(double.Parse(value));
                    break;
                case "scene":
                    scene.UpdateValue(value);
                    break;
                case "id":
                    Index = int.Parse(value);
                    break;
                case "deviceid":
                    Id = value;
                    break;
            }
        }

        internal void Update(string data)
        {
            string[] props = data.Split('&');

            foreach (string prop in props)
            {
                string[] split = prop.Split('=');
                string k = split[0];
                string v = split[1];

                switch (k)
                {
                    case "color":
                        color.UpdateValue(v);
                        break;
                    case "mode":
                        mode.UpdateValue(v);
                        break;
                    case "power":
                        power.UpdateValue(bool.Parse(v));
                        break;
                    case "brightness":
                        brightness.UpdateValue(double.Parse(v));
                        break;
                    case "colortemp":
                        colorTemp.UpdateValue(double.Parse(v));
                        break;
                    case "id":
                        Index = int.Parse(v);
                        break;
                    case "deviceid":
                        Id = v;
                        break;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
