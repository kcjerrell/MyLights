using MyLights.Models;
using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

using WeakBrush = System.WeakReference<System.Windows.Media.SolidColorBrush>;



namespace MyLights.Util
{
    public class BrushCache : IValueConverter
    {
        private static Dictionary<Color, WeakBrush> cache = new Dictionary<Color, WeakBrush>();

        public static SolidColorBrush GetBrush(Color color)
        {
            SolidColorBrush brush;

            if (!cache.ContainsKey(color))
            {
                brush = new SolidColorBrush(color);
                var weakref = new WeakBrush(brush);
                cache[color] = weakref;
            }

            else if (!cache[color].TryGetTarget(out brush))
            {
                brush = new SolidColorBrush(color);
                cache[color].SetTarget(brush);
            }

            return brush;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color outColor;
            if (value is Color color)
            {
                if (parameter is string param)
                {
                    if (param.ToLower() == "fullv")
                    {
                        Helpers.ColorToHSV(color, out double h, out double s, out double v);
                        outColor = Helpers.ColorFromHSV(h, s, 1.0);
                    }
                }
                else
                {
                    outColor = color;
                }

                return GetBrush(outColor);
            }

            else if (value is HSV hsv)
            {
                if (parameter is string param)
                {
                    if (param.ToLower() == "fullv")
                    {
                        outColor = new HSV(hsv.H, hsv.S, 1.0).ToColor();
                    }
                }
                else
                {
                    outColor = hsv.ToColor();
                }

                return GetBrush(outColor);
            }

            else if (value is double colorTemp)
            {
                if (parameter is string param && param == "colortemp")
                {
                    return GetBrush(Helpers.ColorTempToColor(colorTemp));
                }
            }

            else if (value is LightState ls)
            {
                if (!ls.Power)
                {
                    return GetBrush(Color.FromRgb(34, 34, 34));
                }
                else if (ls.Mode == LightMode.Color)
                {
                    outColor = ls.Color.V(1).ToColor();
                    return GetBrush(outColor);
                }
                else if (ls.Mode == LightMode.White)
                {
                    return GetBrush(Helpers.ColorTempToColor(ls.ColorTemp));
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
