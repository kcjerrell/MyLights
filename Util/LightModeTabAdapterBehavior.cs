using Microsoft.Xaml.Behaviors;
using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace MyLights.Util
{
    public class LightModeTabAdapterBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;
        }
        protected override void OnDetaching()
        {
            AssociatedObject.IsVisibleChanged -= AssociatedObject_IsVisibleChanged;
        }

        private void AssociatedObject_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (AssociatedObject.DataContext is LightViewModel lvm)
            {
                if (AssociatedObject.IsVisible)
                    lvm.Mode = "colour";
                else
                    lvm.Mode = "white";
            }
        }


    }
}
