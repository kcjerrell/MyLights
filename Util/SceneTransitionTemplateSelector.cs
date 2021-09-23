using MyLights.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyLights.Util
{
    public class SceneTransitionTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is SceneTransition trans)
            {
                return trans switch
                {
                    SceneTransition.Static => Static,
                    SceneTransition.Flash => Flash,
                    SceneTransition.Breath => Breath,
                    _ => null
                };
            }

            return null;
        }

        public DataTemplate Static { get; set; }
        public DataTemplate Flash { get; set; }
        public DataTemplate Breath { get; set; }
    }
}
