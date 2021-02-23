using System;
using System.Globalization;
using System.Windows.Data;

namespace MyLights.Util
{
    class BoolToObjectsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                if (b)
                    return TrueValue;
                else
                    return FalseValue;
            }

            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object FalseValue { get; set; }

        public object TrueValue { get; set; }
    }
}
