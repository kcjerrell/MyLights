using MyLights.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyLights.Controls
{
    public class ColorSpectrum1D : FrameworkElement
    {
        WriteableBitmap bitmap;

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
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
        public bool IsReversed
        {
            get { return (bool)GetValue(IsReversedProperty); }
            set { SetValue(IsReversedProperty, value); }
        }
        public HSVComponent Mode
        {
            get { return (HSVComponent)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }
        public bool IsHorizontal { get => Orientation == Orientation.Horizontal; }

        private void CreateBitmap()
        {
            int w = Math.Max(1, (int)RenderSize.Width);
            int h = Math.Max(1, (int)RenderSize.Height);

            if (IsHorizontal)
                h = 1;

            else
                w = 1;

            bitmap = new WriteableBitmap(w, h, 96, 96, PixelFormats.Bgr32, null);

            GenerateSpectrum();
        }

        private void GenerateSpectrum()
        {
            if (bitmap == null)
                return;

            int pixels = Math.Max(bitmap.PixelWidth, bitmap.PixelHeight);

            int mode = Mode switch
            {
                HSVComponent.Hue => 1,
                HSVComponent.Saturation => 2,
                HSVComponent.Value => 3,
                _ => 4
            };

            try
            {
                bitmap.Lock();

                byte r = 0, g = 0, b = 0;

                unsafe
                {
                    IntPtr bufPtr = bitmap.BackBuffer;

                    for (int i = 0; i < pixels; i++)
                    {
                        double x = (double)i / pixels;

                        if (mode == 1)
                            Helpers.HsvToRgb(x, Saturation, Value, out r, out g, out b);
                        else if (mode == 2)
                            Helpers.HsvToRgb(Hue, x, Value, out r, out g, out b);
                        else if (mode == 3)
                            Helpers.HsvToRgb(Hue, Saturation, x, out r, out g, out b);

                        int pixel = 255 << 24;
                        pixel |= r << 16;
                        pixel |= g << 8;
                        pixel |= b << 0;

                        *((int*)bufPtr) = pixel;

                        bufPtr += 4;
                    }
                }

                bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            }

            finally
            {
                bitmap.Unlock();
            }
        }

        #region Overrides
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (bitmap == null)
                CreateBitmap();

            drawingContext.DrawImage(bitmap, new Rect(0.0, 0.0, RenderSize.Width, RenderSize.Height));

            base.OnRender(drawingContext);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            CreateBitmap();
            base.OnRenderSizeChanged(sizeInfo);
        }

        #endregion


        #region DependencyProperty callbacks
        private void OnHSVChanged(DependencyPropertyChangedEventArgs e)
        {
            GenerateSpectrum();
            InvalidateVisual();
        }

        private void OnOrientationChanged(DependencyPropertyChangedEventArgs e)
        {
            CreateBitmap();
            InvalidateVisual();
        }

        private void OnIsReversedChanged(DependencyPropertyChangedEventArgs e)
        {
            GenerateSpectrum();
            InvalidateVisual();
        }

        private void OnModeChanged(DependencyPropertyChangedEventArgs e)
        {
            GenerateSpectrum();
            InvalidateVisual();
        }

        #endregion


        #region Static
        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register("Hue", typeof(double), typeof(ColorSpectrum1D),
                new PropertyMetadata(1.0, (s, e) => ((ColorSpectrum1D)s).OnHSVChanged(e)));

        public static readonly DependencyProperty SaturationProperty =
            DependencyProperty.Register("Saturation", typeof(double), typeof(ColorSpectrum1D),
                new PropertyMetadata(1.0, (s, e) => ((ColorSpectrum1D)s).OnHSVChanged(e)));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ColorSpectrum1D),
                new PropertyMetadata(1.0, (s, e) => ((ColorSpectrum1D)s).OnHSVChanged(e)));

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ColorSpectrum1D),
                new PropertyMetadata(Orientation.Horizontal, (s, e) => ((ColorSpectrum1D)s).OnOrientationChanged(e)));

        public static readonly DependencyProperty IsReversedProperty =
            DependencyProperty.Register("IsReversed", typeof(bool), typeof(ColorSpectrum1D),
                new PropertyMetadata(false, (s, e) => ((ColorSpectrum1D)s).OnIsReversedChanged(e)));

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(HSVComponent), typeof(ColorSpectrum1D),
                new PropertyMetadata(HSVComponent.Hue, (s, e) => ((ColorSpectrum1D)s).OnModeChanged(e)));
        #endregion
    }

    public enum HSVComponent
    {
        Hue,
        Saturation,
        Value
    }
}
