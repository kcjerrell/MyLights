using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MyLights.Util
{
    class MathConverter : IValueConverter
    {
        /// <summary>
        /// For the parameter, a string with the desired operation followed by the operand
        /// 
        /// For example:
        /// 
        /// "*10.0"
        /// "+2"
        /// 
        /// strings that don't fit this format exactly will probably throw
        /// 
        /// it will probably throw anyway!
        /// 
        /// Also, it only takes * and +
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = (double)value;
            if (parameter is string p)
            {
                char op = p[0];
                if (double.TryParse(p.Substring(1), out double operand))
                {
                    //switch (op)
                    //{
                    //    case '*':
                    //        val = val * operand;
                    //        break;
                    //    case '+':
                    //        val = val + operand;
                    //        break;
                    //    default:
                    //        break;
                    //}

                    val = op switch
                    {
                        '*' => val * operand,
                        '+' => val + operand,
                        _ => val,
                    };
                }
            }
            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
