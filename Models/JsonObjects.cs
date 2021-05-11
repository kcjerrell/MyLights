using MyLights.Bridges;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

    public class JsonBulb : ILightPropertiesProvider
    {
        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("name")]
        public string Id { get; set; }

        [JsonProperty("power")]
        public bool Power { get; set; }

        [JsonProperty("color")]
        public HSV Color { get; set; }

        [JsonProperty("mode")]
        public LightMode Mode { get; set; }

        [JsonProperty("colortemp")]
        public int ColorTemp { get; set; }

        [JsonProperty("brightness")]
        public int Brightness { get; set; }

        public IDeviceProperty<bool> PowerProperty => new DpsPower(Index.ToString(), Power);

        public IDeviceProperty<HSV> ColorProperty => new DpsColor(Index.ToString(), Color);

        public IDeviceProperty<LightMode> ModeProperty => new DpsMode(Index.ToString(), Mode);

        public IDeviceProperty<double> BrightnessProperty => new DpsBrightness(Index.ToString(), Brightness);

        public IDeviceProperty<double> ColorTempProperty => new DpsColorTemp(Index.ToString(), ColorTemp);

        //public string whatever { get; set; }
    }

    public class JsonBulbRoot
    {
        public List<JsonBulb> bulbs { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class JsonDpsX
    {
        [JsonProperty("20")]
        public bool Power { get; set; }

        [JsonProperty("21")]
        public string Mode { get; set; }

        [JsonProperty("22")]
        public int Brightness { get; set; }

        [JsonProperty("23")]
        public int ColorTemp { get; set; }

        [JsonProperty("24")]
        public HSV Color { get; set; }

        [JsonProperty("25")]
        public string Scene { get; set; }

        [JsonProperty("26")]
        public int Timer { get; set; }
    }

    public class JsonDatum
    {
        [JsonProperty("dps")]
        public JsonDps Dps { get; set; }
    }

    public class JsonDpsRoot
    {
        [JsonProperty("data")]
        public List<JsonDatum> Data { get; set; }
    }

    public class JsonDps
    {
        [JsonProperty("power")]
        public bool Power { get; set; }

        [JsonProperty("color")]
        public HSV Color { get; set; }

        [JsonProperty("mode")]
        public LightMode Mode { get; set; }
    }
}
