using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Flurl;
using Flurl.Http;
using MyLights.Util;

namespace MyLights.Models
{
    public class Light
    {
        public Light(JsonBulb jBulb)
        {
            this.jsonBulb = jBulb;
            this.Index = jBulb.index;
            string index = jBulb.index.ToString();
            
            this.Name = jBulb.name;

            color = new DpsColor(index, jBulb.color);
            mode = new DpsMode(index, jBulb.mode);
            power = new DpsPower(index, jBulb.power);
            brightness = new DpsBrightness(index, jBulb.brightness);
            colorTemp = new DpsColorTemp(index, jBulb.colortemp);
        }

        JsonBulb jsonBulb;

        DpsColor color;
        DpsMode mode;
        DpsPower power;
        DpsBrightness brightness;
        DpsColorTemp colorTemp;

        public int Index { get; private set; }
        public string Name { get; private set; }

        public HSV Color { get => color.Value; }
        public bool Power { get => power.Value; }
        public string Mode { get => mode.Value; }
        public double Brightness { get => brightness.Value; }
        public double ColorTemp { get => colorTemp.Value; }

        // #lightgroup 
        public LightGroup Group { get; private set; }

        private bool isLead;

        public void Engroup(LightGroup group, bool isLead)
        {
            this.Group = group;
            this.isLead = isLead;
        }

        internal void Ungroup()
        {
            this.Group = null;
            this.isLead = false;
        }

        public void SetColor(HSV value)
        {
            color.Set(value);
        }

        public void SetPower(bool value)
        {
            power.Set(value);
        }

        public void SetMode(string value)
        {
            mode.Set(value);
        }
    }
}
