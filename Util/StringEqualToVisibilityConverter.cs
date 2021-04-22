using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MyLights.Util
{
    class StringEqualToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string valueText && parameter is string matchText)
            {
                if (valueText == matchText)
                {
                    return DefaultVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
                }
            }

            return DefaultVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }



        public string MatchText { get; set; } = "";

        public Visibility DefaultVisibility { get; set; } = Visibility.Collapsed;


    }
}
