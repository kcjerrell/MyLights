using MyLights.Models;
using System;

namespace MyLights.ViewModels
{
    public class BulbSetter
    {
        public bool Power { get; set; }
        public HSV Color{ get; set; }

        internal void Activate(Light light)
        {
            light.SetColor(Color);
            light.SetPower(true);
        }
    }
}