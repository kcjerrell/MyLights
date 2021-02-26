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

        public static Color ToColor(this JsonColor jsonColor)
        {
            return Color.FromRgb((byte)jsonColor.r, (byte)jsonColor.g, (byte)jsonColor.b);
        }

    }
}
