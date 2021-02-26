﻿using MyLights.Util;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows.Media;
using System;

namespace MyLights.Models
{
    public struct HSV : IEquatable<HSV>
    {
        [JsonProperty("h")]
        public double H { get; set; }
        [JsonProperty("s")]
        public double S { get; set; }
        [JsonProperty("v")]
        public double V { get; set; }

        public HSV(double h, double s, double v)
        {
            H = h; S = s; V = v;
        }

        public Color ToColor()
        {
            Helpers.HsvToRgb(H, S, V, out int r, out int g, out int b);
            return Color.FromRgb((byte)r, (byte)g, (byte)b);
        }

        public static HSV FromColor(Color color)
        {
            Helpers.ColorToHSV(color, out double h, out double s, out double v);
            return new HSV(h, s, v);
        }


        public override bool Equals(object obj)
        {
            if (obj is HSV other)
                return Equals(other);
            else
                return false;
        }

        public bool Equals(HSV other)
        {
            return this.H == other.H &&
                   this.S == other.S &&
                   this.V == other.V;
        }

        public static bool operator ==(HSV left, HSV right)
        {
            return left.H == right.H &&
                   left.S == right.S &&
                   left.V == right.V;
        }

        public static bool operator !=(HSV left, HSV right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return (int)(H * 994000000 + S * 993989 + V * 13);
        }
    }
}
