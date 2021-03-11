using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

    public class JsonBulb
    {
        public int index { get; set; }
        public string name { get; set; }
        public string whatever { get; set; }
        public bool power { get; set; }
        public HSV color { get; set; }
        public string mode { get; set; }
        public int colortemp { get; set; }
        public int brightness { get; set; }
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
        public string Mode { get; set; }
    }
}
