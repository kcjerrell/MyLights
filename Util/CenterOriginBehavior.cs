using System;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace MyLights.Util
{
    public class CenterOriginBehavior : Behavior<FrameworkElement>
    {
        TranslateTransform transform;

        protected override void OnAttached()
        {
            UpdateTransform();
            AssociatedObject.SizeChanged += (s, e) => UpdateTransform();
        }

        private void UpdateTransform()
        {
            if (AssociatedObject.RenderTransform != transform)
            {
                transform = new TranslateTransform();
                AssociatedObject.RenderTransform = transform;
            }

            transform.X = AssociatedObject.ActualWidth / -2.0;
            transform.Y = AssociatedObject.ActualHeight / -2.0;
        }

        protected override void OnDetaching()
        {
            throw new Exception("why is this being detached?");
        }
    }
}
