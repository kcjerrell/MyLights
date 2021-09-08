using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MyLights.Util
{
    public class AnimationFactory
    {
        public AnimationFactory()
        {
            sb = new Storyboard();
        }

        private Storyboard sb;

        public void AddDoubleAnimation(DependencyObject target, string property, double value, double seconds)
        {
            var anim = new DoubleAnimation()
            {
                To = value,
                Duration = DurationFromSeconds(seconds),
            };
            SetTarget(target, property, anim);

            sb.Children.Add(anim);
        }

        public void AddSetValue(DependencyObject target, string property, object value)
        {
            var anim = new ObjectAnimationUsingKeyFrames();
            var kf = new DiscreteObjectKeyFrame()
            {
                Value = value
            };
            anim.KeyFrames.Add(kf);
            SetTarget(target, property, anim);

            sb.Children.Add(anim);
        }

        public void AddColorAnimation(DependencyObject target, string property, Color color, double seconds)
        {
            var anim = new ColorAnimation()
            {
                To = color,
                Duration = DurationFromSeconds(seconds)
            };
            SetTarget(target, property, anim);

            sb.Children.Add(anim);
        }

        private static void SetTarget(DependencyObject target, string property, Timeline anim)
        {
            Storyboard.SetTarget(anim, target);
            Storyboard.SetTargetProperty(anim, new PropertyPath(property));
        }

        private static Duration DurationFromSeconds(double seconds)
        {
            return new Duration(TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// Returns the constructed storyboard, and starts a new one
        /// </summary>
        /// <returns></returns>
        public Storyboard Release()
        {
            Storyboard r = sb;
            sb = new Storyboard();
            return r;
        }
    }
}