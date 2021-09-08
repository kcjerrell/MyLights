using System;

namespace MyLights.Models
{
    public class LightState
    {
        public LightState()
        {
        }

        public LightState(Light light)
        {
            this.Power = light.Power;
            this.Color = light.Color;
            this.Mode = light.Mode;
            this.Brightness = light.Brightness;
            this.ColorTemp = light.ColorTemp;
        }

        public bool Power { get; set; }
        public HSV Color { get; set; }
        public LightMode Mode { get; set; }
        public double Brightness { get; set; }
        public double ColorTemp { get; set; }

        internal void Apply(Light light)
        {
            light.SetPower(Power);

            if (Power)
            {
                light.SetMode(Mode);

                if (Mode == LightMode.Color)
                {
                    light.SetColor(Color);
                }
                else if (Mode == LightMode.White)
                {
                    light.SetBrightness(Brightness);
                    light.SetColorTemp(ColorTemp);
                }
            }
        }
    }
}