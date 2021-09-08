using MyLights.Models;
using MyLights.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyLights.Controls
{
    public class ColorSlider : Control
    {
        public ColorSlider()
        {
            Loaded += ColorSlider_Loaded;
        }

        FrameworkElement thumb;
        TranslateTransform transform;

        ColorSpectrum1D spectrum;
        ColorSpectrum1D spectrumBorder;

        bool isDragging = false;

        #region Properties
        public double ThumbOffset
        {
            get
            {
                if (transform == null)
                    return 0.0;

                if (IsHorizontal)
                    return transform.X;
                else
                    return transform.Y;
            }
            set
            {
                if (ThumbOffset != value)
                {
                    if (IsHorizontal)
                        transform.X = value.Clamp(ActualWidth / -2.0, ActualWidth / 2.0);
                    else
                        transform.Y = value.Clamp(ActualHeight / -2.0, ActualHeight / 2.0);

                    UpdateValues();
                }
            }
        }
        public double TrackLength
        {
            get
            {
                if (IsHorizontal)
                    return ActualWidth;
                else
                    return ActualHeight;
            }
        }
        public double Hue
        {
            get { return (double)GetValue(HueProperty); }
            set { SetValue(HueProperty, value); }
        }
        public double Saturation
        {
            get { return (double)GetValue(SaturationProperty); }
            set { SetValue(SaturationProperty, value); }
        }
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public HSVComponent Mode
        {
            get { return (HSVComponent)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }
        public bool IsHorizontal { get => Orientation == Orientation.Horizontal; }
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
        public HSV HSV
        {
            get { return (HSV)GetValue(HSVProperty); }
            private set { SetValue(HSVPropertyKey, value); }
        }
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            private set { SetValue(ColorPropertyKey, value); }
        }
        #endregion


        #region Methods
        private void UpdateValues()
        {
            //CoerceValue(HueProperty);
            //CoerceValue(SaturationProperty);
            //CoerceValue(ValueProperty);
            //CoerceValue(HSVProperty);
            //CoerceValue(ColorProperty);

            double xr = ThumbOffset / TrackLength + 0.5;

            switch (Mode)
            {
                case HSVComponent.Hue:
                    Hue = xr;
                    break;
                case HSVComponent.Saturation:
                    Saturation = xr;
                    break;
                case HSVComponent.Value:
                    Value = xr;
                    break;
                default:
                    break;
            }
        }

        private void MoveThumb(Point point)
        {
            if (IsHorizontal)
                UpdateThumb(point.X / TrackLength);
            else
                UpdateThumb(point.Y / TrackLength);
        }

        private void UpdateThumb(double x)
        {
            double offset = (x - 0.5) * TrackLength;

            if (ThumbOffset != offset)
            {
                ThumbOffset = offset;
            }
        }

        private void UpdateSpectrumBorder()
        {
            if (spectrumBorder != null)
            {
                // spectrumBorder mode is set in binding
                switch (Mode)
                {
                    case HSVComponent.Hue:
                        spectrumBorder.Value = 1.0;
                        spectrumBorder.Saturation = 1.0;
                        break;
                    case HSVComponent.Saturation:
                        spectrumBorder.Hue = 0.0;
                        spectrumBorder.Value = 1.0;
                        break;
                    case HSVComponent.Value:
                        spectrumBorder.Saturation = 0.0;
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion


        #region EventHandlers
        private void ColorSlider_Loaded(object sender, RoutedEventArgs e)
        {
            thumb = (FrameworkElement)GetTemplateChild("HorizontalThumb");
            spectrum = (ColorSpectrum1D)GetTemplateChild("HorizontalSpectrum");
            spectrumBorder = (ColorSpectrum1D)GetTemplateChild("HorizontalSpectrumBorder");

            if (thumb != null)
            {
                //.DragStarted += Thumb_DragStarted;
                //thumb.DragOver += Thumb_DragOver;
                //thumb.DragDelta += Thumb_DragDelta;

                transform = new TranslateTransform();
                thumb.RenderTransform = transform;

                UpdateValues();
            }

            UpdateSpectrumBorder();
        }

        //private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        //{
        //    if (IsHorizontal)
        //    {
        //        ThumbOffset += e.HorizontalChange;
        //    }
        //
        //    else
        //    {
        //        ThumbOffset += e.VerticalChange;
        //    }
        //}
        //
        //private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        //{
        //
        //}
        //
        //private void Thumb_DragOver(object sender, DragEventArgs e)
        //{
        //}
        #endregion


        #region Overrides
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {

            if (CaptureMouse())
            {
                isDragging = true;
                MoveThumb(e.GetPosition(this));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isDragging)
            {
                MoveThumb(e.GetPosition(this));
            }
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            isDragging = false;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
                MoveThumb(e.GetPosition(this));
                isDragging = false;
            }
        }
        #endregion


        #region PropertyChangedCallbacks
        private void OnHueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;

            if (Mode == HSVComponent.Hue)
                UpdateThumb(Hue);

            //CoerceValue(HSVProperty);
            //CoerceValue(ColorProperty);

            HSV = new HSV(Hue, HSV.S, HSV.V);
        }

        private void OnSaturationChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;

            if (Mode == HSVComponent.Saturation)
                UpdateThumb(Saturation);

            //CoerceValue(HSVProperty);
            //CoerceValue(ColorProperty);

            HSV = new HSV(HSV.H, Saturation, HSV.V);
        }

        private void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;

            if (Mode == HSVComponent.Value)
                UpdateThumb(Value);

            //CoerceValue(HSVProperty);
            //CoerceValue(ColorProperty);

            HSV = new HSV(HSV.H, HSV.S, Value);
        }

        private void OnModeChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateSpectrumBorder();
        }

        private void OnOrientationChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private void OnHSVChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;

            Color = HSV.ToColor();

        }
        #endregion


        #region CoerceValueCallbacks
        private object CoerceHue(double e)
        {
            //if (Mode == HSVComponent.Hue)
            //    e = ThumbOffset / TrackLength + 0.5;

            return e.Clamp(0.0, 1.0);
        }
        private object CoerceSaturation(double e)
        {
            //if (Mode == HSVComponent.Saturation)
            //    e = ThumbOffset / TrackLength + 0.5;

            return e.Clamp(0.0, 1.0);
        }
        private object CoerceValue(double e)
        {
            //if (Mode == HSVComponent.Value)
            //    e = ThumbOffset / TrackLength + 0.5;

            return e.Clamp(0.0, 1.0);
        }
        #endregion


        #region Static
        static ColorSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorSlider), new FrameworkPropertyMetadata(typeof(ColorSlider)));
        }

        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register("Hue", typeof(double), typeof(ColorSlider),
                new PropertyMetadata(0.0, (s, e) => ((ColorSlider)s).OnHueChanged(e),
                                             (s, e) => ((ColorSlider)s).CoerceHue((double)e)));

        public static readonly DependencyProperty SaturationProperty =
            DependencyProperty.Register("Saturation", typeof(double), typeof(ColorSlider),
                new PropertyMetadata(1.0, (s, e) => ((ColorSlider)s).OnSaturationChanged(e),
                                             (s, e) => ((ColorSlider)s).CoerceSaturation((double)e)));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ColorSlider),
                new PropertyMetadata(1.0, (s, e) => ((ColorSlider)s).OnValueChanged(e),
                                             (s, e) => ((ColorSlider)s).CoerceValue((double)e)));

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(HSVComponent), typeof(ColorSlider),
                new PropertyMetadata(HSVComponent.Hue, (s, e) => ((ColorSlider)s).OnModeChanged(e)));

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ColorSlider),
                new PropertyMetadata(Orientation.Horizontal, (s, e) => ((ColorSlider)s).OnOrientationChanged(e)));

        private static readonly DependencyPropertyKey HSVPropertyKey =
            DependencyProperty.RegisterReadOnly("HSV", typeof(HSV), typeof(ColorSlider),
                new PropertyMetadata(default(HSV), (s, e) => ((ColorSlider)s).OnHSVChanged(e)));
        public static readonly DependencyProperty HSVProperty = HSVPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey ColorPropertyKey =
            DependencyProperty.RegisterReadOnly("Color", typeof(Color), typeof(ColorSlider),
                new PropertyMetadata(default(Color)));
        public static readonly DependencyProperty ColorProperty = ColorPropertyKey.DependencyProperty;

        #endregion
    }
}
