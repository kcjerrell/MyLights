using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

using TAC = System.Windows.Interactivity.TriggerActionCollection;

namespace MyLights.Util
{
    public class CornerRadiusAdjusterBehavior : Behavior<Border>
    {
        private string side = "";
        public string Side
        {
            get => side;
            set
            {
                side = value;
                SetCornerRadius();
            }
        }
        private void SetCornerRadius()
        {
            if (AssociatedObject != null)
            {
                double w = AssociatedObject.ActualWidth;
                double h = AssociatedObject.ActualHeight;

                AssociatedObject.CornerRadius = Side.ToLower() switch
                {
                    "left" => new CornerRadius(h / 2.0, 0.0, 0.0, h / 2.0),
                    "top" => new CornerRadius(w / 2.0, w / 2.0, 0, 0),
                    "right" => new CornerRadius(0.0, h / 2.0, h / 2.0, 0.0),
                    "bottom" => new CornerRadius(0.0, 0.0, w / 2.0, w / 2.0),
                    _ => new CornerRadius()
                };
            }
        }

        protected override void OnAttached()
        {
            AssociatedObject.SizeChanged += AssociatedObject_SizeChanged;
            SetCornerRadius();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SizeChanged -= AssociatedObject_SizeChanged;
        }

        private void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetCornerRadius();
        }


    }
}
