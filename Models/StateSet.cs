﻿using System.Collections.Generic;

namespace MyLights.Models
{
    public class StateSet
    {
        public StateSet(string name)
        {
            this.Name = name;
        }

        public Dictionary<string, LightState> LightStates { get; } = new();
        public string Name { get; set; }

        public void Add(Light light)
        {
            var state = new LightState(light);

            Add(light.Id, state);
        }

        public void Add(string name, LightState state)
        {
            LightStates.Add(name, state);
        }
    }
}