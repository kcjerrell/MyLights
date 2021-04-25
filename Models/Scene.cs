using System.Collections.Generic;

namespace MyLights.Models
{
    public class Scene
    {
        public Scene(string name)
        {
            this.Name = name;
        }

        public Dictionary<string, LightState> LightStates { get; } = new();
        public string Name { get; set; }

        public void Add(Light light)
        {
            var state = new LightState(light);

            LightStates.Add(light.Name, state);
        }
    }
}