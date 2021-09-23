using MyLights.Models;
using MyLights.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using FPMO = System.Windows.FrameworkPropertyMetadataOptions;

namespace MyLights.Controls
{
    // This should be derived from RangeBase.
    public class Knob : Control
    {
        public Knob()
        {
            this.Loaded += Knob_Loaded;
        }

        FrameworkElement notch;
        RotateTransform transform;
        CircleSegment outline;
        CircleSegment inline;
        double zeroX;
        Point lastMousePos;
        DateTime lastWheel;
        SolidColorBrush outlineBrush;
        Brush originalOutlineBrush;

        const double valueToPixelRatio = 200.0;
        const double mouseWheelMultiplier = 0.25;
        const double arcLength = 280.0;


        #region Properties
        [CategoryAttribute("Common")]
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        [CategoryAttribute("Common")]
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        [CategoryAttribute("Common")]
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        [CategoryAttribute("Common")]
        public KnobControlStyle ControlStyle
        {
            get { return (KnobControlStyle)GetValue(ControlStyleProperty); }
            set { SetValue(ControlStyleProperty, value); }
        }

        [CategoryAttribute("Appearance")]
        public bool ShowOutline
        {
            get { return (bool)GetValue(ShowOutlineProperty); }
            set { SetValue(ShowOutlineProperty, value); }
        }

        [CategoryAttribute("Appearance")]
        public bool ColorOutline
        {
            get { return (bool)GetValue(ColorOutlineProperty); }
            set { SetValue(ColorOutlineProperty, value); }
        }
        #endregion

        #region Methods
        private void PositionComponents()
        {
            if (notch != null && transform != null)
            {
                double x = (RenderSize.Width - notch.ActualWidth) / 2.0;
                double y = RenderSize.Height - notch.ActualHeight - inline.Margin.Bottom;

                Canvas.SetLeft(notch, x);
                Canvas.SetTop(notch, y);

                transform.CenterX = ActualWidth / 2.0;
                transform.CenterY = ActualHeight / 2.0;
            }
        }

        private void UpdateComponents()
        {
            // notch transform angle runs from 15 to 345
            // circle seg arc length runs from 0 to 330

            double r = (Value - Minimum) / (Maximum - Minimum);

            if (transform != null)
                transform.Angle = r * arcLength + (360.0 - arcLength) / 2.0;

            if (outline != null)
            {
                outline.ArcLength = r * arcLength;

                if (ColorOutline)
                {
                    HSV hsv = new HSV(r * -0.3 + 0.3, 1, 1);
                    outlineBrush.Color = hsv.ToColor();
                }
            }
        }

        private void NudgeValue(double deltaX, double deltaY)
        {
            // double dx = position.X - clickOriginZero.X;
            //double dx = position.X - zeroX;
            //double r = dx.Clamp(0, valueToPixelRatio) / valueToPixelRatio;
            //Value = r * (Maximum - Minimum) + Minimum;

            double change = (deltaX - deltaY) / 2.0 / valueToPixelRatio;
            Value += change * (Maximum - Minimum);
        }

        private void SetValue(double r)
        {
            double range = Maximum - Minimum;
            Value = r * range + Minimum;
        }
        #endregion


        #region EventHandlers
        private void Knob_Loaded(object sender, RoutedEventArgs e)
        {
            notch = (FrameworkElement)GetTemplateChild("Notch");
            transform = (RotateTransform)GetTemplateChild("NotchTransform");
            outline = (CircleSegment)GetTemplateChild("Outline");
            inline = (CircleSegment)GetTemplateChild("Inline");

            if (ColorOutline)
            {
                originalOutlineBrush = outline.Stroke;
                outlineBrush = new SolidColorBrush();
                outline.Stroke = outlineBrush;
            }

            PositionComponents();
            UpdateComponents();
        }
        #endregion


        #region Overrides
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            PositionComponents();

            return base.ArrangeOverride(arrangeBounds);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(this);
            double r = (Value - Minimum) / (Maximum - Minimum);

            //clickOriginZero = new Point(pos.X - r * valueToPixelRatio, pos.Y - r * valueToPixelRatio);
            zeroX = pos.X - r * valueToPixelRatio;

            lastMousePos = e.GetPosition(this);

            CaptureMouse();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                Point pos = e.GetPosition(this);

                switch (ControlStyle)
                {
                    case KnobControlStyle.X:
                        NudgeValue(pos.X - lastMousePos.X, 0);
                        break;
                    case KnobControlStyle.Y:
                        NudgeValue(0, pos.Y - lastMousePos.Y);
                        break;
                    case KnobControlStyle.XY:
                        NudgeValue(pos.X - lastMousePos.X, pos.Y - lastMousePos.Y);
                        break;
                    case KnobControlStyle.Radial:

                        break;
                    default:
                        break;
                }

                Point center = new Point(RenderSize.Width / 2.0, RenderSize.Height / 2.0);
                double angle = 180.0 * (Math.PI - Math.Atan2(center.X - pos.X, center.Y - pos.Y)) / Math.PI;
                double zero = (360.0 - arcLength) / 2.0;
                angle = angle.Clamp(zero, 360.0 - zero) - zero;

                lastMousePos = pos;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {

        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            DateTime proc = DateTime.Now;

            if (proc - lastWheel > TimeSpan.FromMilliseconds(250))
            {
                NudgeValue(e.Delta * mouseWheelMultiplier / 3.0, 0);
            }
            else
            {
                NudgeValue(e.Delta * mouseWheelMultiplier, 0);
            }

            lastWheel = proc;
        }
        #endregion


        #region PropertyChangedCallbacks
        private void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateComponents();
        }
        private void OnMinimumChanged(DependencyPropertyChangedEventArgs e)
        {
            CoerceValue(MaximumProperty);
            CoerceValue(ValueProperty);
            UpdateComponents();
        }
        private void OnMaximumChanged(DependencyPropertyChangedEventArgs e)
        {
            CoerceValue(ValueProperty);
            UpdateComponents();
        }
        private void OnColorOutlineChanged(DependencyPropertyChangedEventArgs e)
        {
            if (outline != null)
            {
                if (ColorOutline)
                {
                    originalOutlineBrush = outline.Stroke;
                    outlineBrush = new SolidColorBrush();
                    outline.Stroke = outlineBrush;
                }
                else
                {
                    outline.Stroke = originalOutlineBrush;
                }
            }
        }
        #endregion




        #region CoerceValueCallbacks
        private object CoerceKnobValue(double e)
        {
            return e.Clamp(Minimum, Maximum);
        }
        private object CoerceMinimum(double e)
        {
            return e;
        }
        private object CoerceMaximum(double e)
        {
            return Math.Max(e, Minimum);
        }
        #endregion


        #region Static
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(Knob),
                new PropertyMetadata(50.0, (s, e) => ((Knob)s).OnValueChanged(e),
                                         (s, e) => ((Knob)s).CoerceKnobValue((double)e)));

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(Knob),
                new PropertyMetadata(0.0, (s, e) => ((Knob)s).OnMinimumChanged(e),
                                           (s, e) => ((Knob)s).CoerceMinimum((double)e)));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(Knob),
                new PropertyMetadata(100.0, (s, e) => ((Knob)s).OnMaximumChanged(e),
                                           (s, e) => ((Knob)s).CoerceMaximum((double)e)));

        public static readonly DependencyProperty ControlStyleProperty =
            DependencyProperty.Register("ControlStyle", typeof(KnobControlStyle), typeof(Knob),
                new PropertyMetadata(KnobControlStyle.X));

        public static readonly DependencyProperty ShowOutlineProperty =
            DependencyProperty.Register("ShowOutline", typeof(bool), typeof(Knob),
                new FrameworkPropertyMetadata(true, FPMO.AffectsRender));

        public static readonly DependencyProperty ColorOutlineProperty =
            DependencyProperty.Register("ColorOutline", typeof(bool), typeof(Knob),
                new FrameworkPropertyMetadata(true, FPMO.AffectsRender, (s, e) => ((Knob)s).OnColorOutlineChanged(e)));

        static Knob()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Knob), new FrameworkPropertyMetadata(typeof(Knob)));
        }

        #endregion
    }

    public enum KnobControlStyle
    {
        X,
        Y,
        XY,
        Radial
    }
}
