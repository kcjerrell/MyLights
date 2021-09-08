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
    public static partial class Extensions
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

        public static int Clamp(this int x, int min, int max)
        {
            if (max < min)
                throw new ArgumentException("get your shit straight");

            return Math.Min(Math.Max(x, min), max);
        }

        public static double PlusOrMinus(this double x, double maxVariance)
        {
            return x + (Locator.Get.Rand.NextDouble() - 0.5) * maxVariance * 2;
        }

        public static int PlusOrMinus(this int x, int maxVariance)
        {
            return x + Locator.Get.Rand.Next(-1 * maxVariance, maxVariance);
        }

        public static int IncWrap(this int i, int max)
        {
            return (i + 1) % max;
        }

        public static HSV ToHSV(this Color color)
        {
            Helpers.ColorToHSV(color, out double h, out double s, out double v);
            return new HSV(h, s, v);
        }

        public static double MapRange(this double x, double localMin, double localMax, double targetMin, double targetMax)
        {
            x = (x.Clamp(localMin, localMax));
            double xr = (x - localMin) / (localMax - localMin);

            double y = xr * (targetMax - targetMin) + targetMin;

            return y;
        }

        public static IEnumerable<T> SingleEnumerator<T>(this T item)
        {
            yield return item;
        }

        public static bool IsNaN(this double x)
        {
            return double.IsNaN(x);
        }

        public static IEnumerable<T> Without<T>(this IEnumerable<T> source, T excludeItem)
        {
            foreach (T item in source)
            {
                if (!item.Equals(excludeItem))
                    yield return item;
            }
        }
    }
}
