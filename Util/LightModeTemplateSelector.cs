using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyLights.Util
{
    public class LightModeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is LightModes mode)
            {
                return mode switch
                {
                    //LightModes.Off => Off,
                    LightModes.Color => Color,
                    LightModes.White => White,
                    _ => null
                };
            }

            if (item is string text)
            {
                if (text == "color")
                    return Color;
                else if (text == "white")
                    return White;
            }
            return null;
        }

        public DataTemplate Off { get; set; }

        public DataTemplate Color { get; set; }

        public DataTemplate White { get; set; }

    }
}
