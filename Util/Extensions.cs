using MyLights.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MyLights.Util
{
    public static class Extensions
    {
        public static double DistanceFrom(this Point origin, Point other)
        {
            return Math.Sqrt(Math.Pow(other.X - origin.X, 2) + Math.Pow(other.Y - origin.Y, 2));
        }

        public static T[] Fill<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }

            return array;
        }

        public static string Capitalize(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            else if (text.Length == 1)
                return text.ToUpper();

            return text.Substring(0, 1).ToUpper() + text.Substring(1);
        }

        public static double Clamp(this double x, double min, double max)
        {
            //what if max is less than min? throw or just clamp backwards?
            //throw for now

            if (max < min)
                throw new ArgumentException("get your shit straight");

            return Math.Min(Math.Max(x, min), max);
        }

        public static HSV ToHSV(this Color color)
        {
            Helpers.ColorToHSV(color, out double h, out double s, out double v);
            return new HSV(h, s, v);
        }
    }
}
