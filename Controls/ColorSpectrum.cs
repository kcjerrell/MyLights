using MyLights.Models;
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
    public class ColorSpectrum : FrameworkElement
    {
        public ColorSpectrum()
        {
            CreateBitmap(100, 100);
        }

        WriteableBitmap bitmap;

        public Color GetColor(double x, double y, double v)
        {
            double h = x / ActualWidth;
            double s = y / ActualHeight;

            Helpers.HsvToRgb(h * 360, s, v, out byte r, out byte g, out byte b);

            return Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawImage(bitmap, new Rect(0, 0, RenderSize.Width, RenderSize.Height));
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            //double w = availableSize.Width == double.PositiveInfinity ? 50 : availableSize.Width;
            //double h = availableSize.Height == double.PositiveInfinity ? 50 : availableSize.Height;

            return new Size(0, 0);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return finalSize;
        }

        private void CreateBitmap(int width, int height)
        {
            bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
            GenerateSpectrum(bitmap, Mode, ThirdValue);
        }

        public SpectrumModes Mode
        {
            get { return (SpectrumModes)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(SpectrumModes), typeof(ColorSpectrum),
                new PropertyMetadata(SpectrumModes.HS, (s, e) => ((ColorSpectrum)s).OnModeChanged(e)));

        private void OnModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (bitmap != null)
            {
                GenerateSpectrum(bitmap, Mode, ThirdValue);
            }
        }

        public double ThirdValue
        {
            get { return (double)GetValue(ThirdValueProperty); }
            set { SetValue(ThirdValueProperty, value); }
        }

        public static readonly DependencyProperty ThirdValueProperty =
            DependencyProperty.Register("ThirdValue", typeof(double), typeof(ColorSpectrum),
                new PropertyMetadata(1.0, (s, e) => ((ColorSpectrum)s).OnThirdValueChanged(e)));

        private void OnThirdValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (bitmap != null)
            {
                GenerateSpectrum(bitmap, Mode, ThirdValue);
            }
        }


        private static HSV ModeConversion(SpectrumModes mode, double a, double b, double c)
        {
            return mode switch
            {
                SpectrumModes.HS => new HSV(a, b, c),
                SpectrumModes.HV => new HSV(a, c, b),
                SpectrumModes.SV => new HSV(c, a, b),
                _ => new HSV()
            };
        }

        private static void GenerateSpectrum(WriteableBitmap bitmap, SpectrumModes mode, double thirdValue)
        {
            try
            {
                bitmap.Lock();

                unsafe
                {
                    int x, y;
                    double a, b, c;

                    int pixels = bitmap.PixelWidth * bitmap.PixelHeight;

                    c = thirdValue;

                    IntPtr bufPtr = bitmap.BackBuffer;

                    for (int i = 0; i < pixels; i++)
                    {
                        x = i % bitmap.PixelWidth;
                        y = i / bitmap.PixelWidth;

                        a = (double)x / bitmap.PixelWidth;
                        b = (double)y / bitmap.PixelHeight;

                        HSV hsv = ModeConversion(mode, a, b, c);
                        Helpers.HsvToRgb(hsv.H, hsv.S, hsv.V, out byte red, out byte green, out byte blue);

                        int pixel = 255 << 24; // A
                        pixel |= red << 16;
                        pixel |= green << 8;
                        pixel |= blue << 0;

                        *((int*)bufPtr) = pixel;

                        bufPtr += 4; // or is it 4? We'll see.
                    }
                }

                bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            }
            finally
            {
                bitmap.Unlock();
            }
        }
    }

    public enum SpectrumModes
    {
        HS,
        HV,
        SV,
    }
}
