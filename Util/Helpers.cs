using MyLights.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyLights.Util
{
    internal static partial class Helpers
    {
        internal static Color ColorTempToColor(double colorTemp)
        {
            double x = colorTemp.Clamp(0, 1000) / 1000.0;
            double rg = 255 - x * 69;
            double b = 208 + x * 32;

            return Color.FromRgb((byte)rg, (byte)rg, (byte)b);
        }
        internal static Color HsvToColor(HSV hsv)
        {
            return HsvToColor(hsv.H, hsv.S, hsv.V);
        }

        internal static Color HsvToColor(double h, double s, double v)
        {
            Helpers.HsvToRgb(h, s, v, out byte r, out byte g, out byte b);
            return Color.FromRgb((byte)r, (byte)g, (byte)b);
        }

        /// <summary>
        /// Takes HSV values and provides equivalent RGB values
        /// 
        /// HSV are expected to be between 0.0 and 1.0. RGB values are 0-255.
        /// </summary>
        /// <param name="h"></param>
        /// <param name="S"></param>
        /// <param name="V"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        internal static void HsvToRgb(double h, double S, double V, out byte r, out byte g, out byte b)
        {
            double H = h * 360.0;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }
            r = ByteClamp((int)(R * 255.0));
            g = ByteClamp((int)(G * 255.0));
            b = ByteClamp((int)(B * 255.0));
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        static byte ByteClamp(int i)
        {
            if (i < 0) i = 0;
            if (i > 255) i = 255;
            return (byte)i;
        }

        //https://stackoverflow.com/a/1626175/839853
        public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            var col = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);        

            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = col.GetHue() / 360.0;
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        //https://stackoverflow.com/a/1626175/839853
        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            byte v = Convert.ToByte(value);
            byte p = Convert.ToByte(value * (1 - saturation));
            byte q = Convert.ToByte(value * (1 - f * saturation));
            byte t = Convert.ToByte(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }
    }
}
