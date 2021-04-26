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
    class StringEqualToObjectsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return value;

            string valueText = IgnoreCase ? value.ToString().ToLower() : value.ToString();
            string paramText = IgnoreCase ? parameter.ToString().ToLower() : parameter.ToString();

            bool stringsEqual = valueText == paramText;

            if (!ReturnEnum)
                return stringsEqual ? TrueValue : FalseValue;

            if (ResolveEnum())
                return stringsEqual ? _trueEnum : _falseEnum;

            return value;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private bool isEnumResolved;

        private object _trueEnum;
        private object _trueValue;
        public object TrueValue
        {
            get
            {
                return _trueValue;
            }
            set
            {
                _trueValue = value;
                isEnumResolved = false;
            }
        }

        private object _falseEnum;
        private object _falseValue;
        public object FalseValue
        {
            get
            {
                return _falseValue;
            }
            set
            {
                _falseValue = value;
                isEnumResolved = false;
            }
        }

        public bool IgnoreCase { get; set; }

        public bool ReturnEnum { get; set; }

        public Type EnumType { get; set; }

        private bool ResolveEnum()
        {
            if (!isEnumResolved)
            {
                if (EnumType != null)
                {
                    if (Enum.TryParse(EnumType, TrueValue.ToString(), out object trueEnum)
                     && Enum.TryParse(EnumType, FalseValue.ToString(), out object falseEnum))
                    {
                        _trueEnum = trueEnum;
                        _falseEnum = falseEnum;
                        isEnumResolved = true;
                    }
                }
            }

            return isEnumResolved;
        }


    }
}
