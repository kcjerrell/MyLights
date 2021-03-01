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
        public Light(JsonBulb jsonBulb)
        {
            this.jsonBulb = jsonBulb;

            this.Color = jsonBulb.color;
            this.Power = jsonBulb.power;
            this.Index = jsonBulb.index;
            this.Name = jsonBulb.name;
            this.Mode = jsonBulb.mode;
        }

        JsonBulb jsonBulb;
        HSV nextColor;
        bool updatingColor = false;

        public HSV Color { get; private set; }
        public bool Power { get; private set; }
        public int Index { get; private set; }
        public string Name { get; private set; }
        public string Mode { get; private set; }
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

        public void SetColor(HSV color)
        {
            SetColor(color.H, color.S, color.V);
        }

        public async void SetColor(double h, double s, double v)
        {
            string indexPath;

            if (Group != null)
            {
                if (isLead)
                {
                    indexPath = Group.Indexes;
                }
                else
                {
                    Color = new HSV(h, s, v);
                    return;
                }
            }
            else
            {
                indexPath = Index.ToString();
            }

            nextColor = new HSV(h, s, v);
            if (!updatingColor)
            {

                //int looped = 0;
                updatingColor = true;
                while (!CloseEnough(this.Color, this.nextColor))
                {
                    //if (looped > 100)
                    //    throw new Exception("whoah too much");

                    string url = @"http://localhost:1337/bulbs"
                        .AppendPathSegment(indexPath)
                        .AppendPathSegment("color")
                        .SetQueryParam("h", nextColor.H)
                        .SetQueryParam("s", nextColor.S)
                        .SetQueryParam("v", nextColor.V);

                    var res = await url.GetJsonAsync<JsonDpsRoot>();

                    this.Color = res.Data[0].Dps.Color;

                    //looped += 1;
                }

                updatingColor = false;
            }
        }

        public async void SetPower(bool power)
        {
            string indexPath;

            if (Group != null)
            {
                if (isLead)
                {
                    indexPath = Group.Indexes;
                }
                else
                {
                    Power = power;
                    return;
                }
            }
            else
            {
                indexPath = Index.ToString();
            }

            if (this.Power == power)
                return;

            string url = @"http://localhost:1337/bulbs"
                            .AppendPathSegment(indexPath)
                            .AppendPathSegment("power")
                            .SetQueryParam("v", power);

            var res = await url.GetJsonAsync<JsonDpsRoot>();

            this.Power = res.Data[0].Dps.Power;
        }

        public async void SetMode(string mode)
        {
            string indexPath;
            if (Group != null)
            {
                if (isLead)
                {
                    indexPath = Group.Indexes;
                }
                else
                {
                    Mode = mode;
                    return;
                }
            }
            else
            {
                indexPath = Index.ToString();
            }

            if (this.Mode == mode)
                return;

            string url = @"http://localhost:1337/bulbs"
                .AppendPathSegment(indexPath)
                .AppendPathSegment("mode")
                .SetQueryParam("v", mode);

            var res = await url.GetJsonAsync<JsonDpsRoot>();

            this.Power = res.Data[0].Dps.Power;
        }

        private bool CloseEnough(HSV a, HSV b)
        {
            if (Math.Abs(a.H - b.H) > 0.01)
                return false;
            if (Math.Abs(a.S - b.S) > 0.01)
                return false;
            if (Math.Abs(a.V - b.V) > 0.01)
                return false;

            return true;
        }
    }
}
