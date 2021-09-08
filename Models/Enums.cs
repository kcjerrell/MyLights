using MyLights.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        Scene = 4,
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
        Scene = 25,
        Hue = 240,
        Saturation = 241,
        Value = 242,
    }

    public class LightModeEqualityComparer : EqualityComparer<LightMode>
    {
        public override bool Equals(LightMode x, LightMode y)
        {
            return x == y;
        }

        public override int GetHashCode([DisallowNull] LightMode obj)
        {
            return (int)obj;
        }
    }
}

namespace MyLights.Util
{
    internal static partial class Helpers
    {
        public static string ModeToString(LightMode mode)
        {
            return mode switch
            {
                LightMode.None => "",
                LightMode.Color => "colour",
                LightMode.White => "white",
                LightMode.Music => "music",
                LightMode.Scene => "scene",
                _ => ""
            };
        }

        public static LightMode StringToMode(string mode)
        {
            return mode switch
            {
                "colour" => LightMode.Color,
                "color" => LightMode.Color,
                "white" => LightMode.White,
                "music" => LightMode.Music,
                "scene" => LightMode.Scene,
                _ => LightMode.None
            };
        }
    }
}