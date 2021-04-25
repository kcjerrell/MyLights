using Newtonsoft.Json;

namespace MyLights
{    
    public class BulbRef
    {
        [JsonIgnore]
        public string IP { get; init; }

        [JsonIgnore]
        public string Id { get; init; }

        [JsonProperty("Name")]
        public string Name { get; init; }
    }
}