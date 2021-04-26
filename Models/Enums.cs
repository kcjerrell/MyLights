using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Models
{
    public enum LightMode
    {
        None = 0,
        White = 1,
        Color = 2,
        Music = 3,
    }

    public enum LightProperties
    {
        None = 0,
        Id = 100,
        Name = 101,
        Power = 20,
        Mode = 21,
        Brightness = 22,
        ColorTemp = 23,
        Color = 24,
        Hue = 240,
        Saturation = 241,
        Value = 242,
    }
}
