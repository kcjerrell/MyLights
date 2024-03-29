﻿using MyLights.Util;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows.Media;
using System;
using MyLights.Models;

namespace MyLights.Models
{
    //this class does no value checking at all
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
            Helpers.HsvToRgb(H, S, V, out byte r, out byte g, out byte b);
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

        public override string ToString()
        {
            return $"(HSV: {H.ToString("F3")}, {S.ToString("F3")}, {V.ToString("F3")})";
        }

        public string ToTuya()
        {
            int h = (int)(H * 360);
            int s = (int)(S * 1000);
            int v = (int)(V * 1000);

            return $"{h.ToString("X4")}{s.ToString("X4")}{v.ToString("X4")}";
        }

        public static HSV FromTuya(string tuya)
        {
            string hh = tuya.Substring(0, 4);
            int h = int.Parse(hh, System.Globalization.NumberStyles.HexNumber);

            string hs = tuya.Substring(4, 4);
            int s = int.Parse(hs, System.Globalization.NumberStyles.HexNumber);

            string hv = tuya.Substring(8, 4);
            int v = int.Parse(hv, System.Globalization.NumberStyles.HexNumber);

            return new HSV(h / 360.0, s / 1000.0, v / 1000.0);
        }
    }
}

namespace MyLights.Util
{
    public static partial class Extensions
    {
        public static HSV Alter(this HSV color, double h = double.NaN, double s = double.NaN, double v = double.NaN)
        {
            return new HSV(h.IsNaN() ? color.H : h, 
                           s.IsNaN() ? color.S : s,
                           v.IsNaN() ? color.V : v);
        }

    }
}
