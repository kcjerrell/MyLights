using MyLights.Util;
using System.Windows.Media;

namespace MyLights.Models
{
    internal struct HSV
    {
        public double H { get; set; }
        public double S { get; set; }
        public double V { get; set; }

        public HSV(double h, double s, double v)
        {
            H = h; S = s; V = v;
        }

        public Color Color
        {
            get
            {
                Helpers.HsvToRgb(H, S, V, out int r, out int g, out int b);
                return Color.FromRgb((byte)r, (byte)g, (byte)b);
            }
        }
    }
}
