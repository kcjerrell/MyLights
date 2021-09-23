using MyLights.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MyLights.Converters
{
    public class TransitionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value.ToString().ToLower())
            {
                case "static":
                case "s":
                    return SceneTransition.Static;

                case "beath":
                case "b":
                    return SceneTransition.Breath;

                case "fade":
                case "f":
                    return SceneTransition.Flash;
                                    
                default:
                    return SceneTransition.None;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }
}
