using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Flurl;
using Flurl.Http;
using MyLights.Util;

namespace MyLights.Models
{
    public class Light
    {
        private JsonBulb jsonBulb;
        private DateTime lastRESTcall;

        private GatedRESTProperty<bool> powerREST;
        private GatedRESTProperty<Color> colorREST;


        public Light(JsonBulb jsonBulb)
        {
            this.jsonBulb = jsonBulb;

            this.Color = jsonBulb.color.ToColor();
            this.Power = jsonBulb.power;
            this.Index = jsonBulb.index;
            this.Name = jsonBulb.name;

            string powerUrl = @"http://localhost:1337/bulbs"
                                .AppendPathSegment(this.Index.ToString())
                                .AppendPathSegment("power");
            Func<bool, string> powerQuery = (b) => "".SetQueryParam("v", b);

            powerREST = new GatedRESTProperty<bool>(powerUrl,powerQuery);
        }


        public Color Color { get; private set; }
        public bool Power { get; private set; }
        public int Index { get; private set; }
        public string Name { get; private set; }

        public async void SetColor(Color color)
        {
            if (this.Color == color)
                return;

            string url = @"http://localhost:1337/bulbs"
                .AppendPathSegment(this.Index.ToString())
                .AppendPathSegment("color")
                .SetQueryParam("r", color.R)
                .SetQueryParam("g", color.G)
                .SetQueryParam("b", color.B);

            var res = await url.GetJsonAsync<JsonDpsRoot>();

            this.Color = res.Data[0].Dps.Color.ToColor();
        }

        public void SetPower(bool power)
        {
            //if (this.Power == power)
            //    return;
            //
            //string url = @"http://localhost:1337/bulbs"
            //                .AppendPathSegment(this.Index.ToString())
            //                .AppendPathSegment("power")
            //                .SetQueryParam("v", power);
            //
            //var res = await url.GetJsonAsync<JsonDpsRoot>();
            //
            //this.Power = res.Data[0].Dps.Power;

            powerREST.SetAndForget(power);

            //I generally think this is a good idea. that is, using another class
            //to limit/schedule calls to lightREST. eitherway, at somewhere in the chain, 
            //from sliding a thumb for for Hue, to the command received by the lightbulb, 
            //I forgot how I was gonna end that sentence, so I probably should stop
            //programming now.

            //at a minimum, changes sent to the bulb should wait until the previous request
            //closes. right?

            
        }

        //-Scenes
        //  -Setters
        //      -Simple
        //      -Cycle
        //  -Timelines
    }
}
