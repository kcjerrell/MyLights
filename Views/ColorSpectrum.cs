using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyLights.Views
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

            HsvToRgb(h * 360, s, v, out int r, out int g, out int b);

            return Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawImage(bitmap, new Rect(0, 0, RenderSize.Width, RenderSize.Height));
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double w = availableSize.Width == double.PositiveInfinity ? 200 : availableSize.Width;
            double h = availableSize.Height == double.PositiveInfinity ? 200 : availableSize.Height;

            return new Size(w, h);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return finalSize;
        }

        private void CreateBitmap(int width, int height)
        {
            bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
            GenerateSpectrum(bitmap);
        }

        private static void GenerateSpectrum(WriteableBitmap bitmap)
        {
            try
            {
                bitmap.Lock();

                unsafe
                {
                    int x, y;
                    double hue, saturation;

                    int pixels = bitmap.PixelWidth * bitmap.PixelHeight;

                    double value = 1.0;

                    IntPtr bufPtr = bitmap.BackBuffer;

                    for (int i = 0; i < pixels; i++)
                    {
                        x = i % bitmap.PixelWidth;
                        y = i / bitmap.PixelWidth;

                        hue = (double)x / bitmap.PixelWidth * 360.0;
                        saturation = (double)y / bitmap.PixelHeight;

                        HsvToRgb(hue, saturation, value, out int r, out int g, out int b);

                        if (y > 500)
                            y += 0;

                        int pixel = 255 << 24; // A
                        pixel |= r << 16;
                        pixel |= g << 8;
                        pixel |= b << 0;

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
        static void HsvToRgb(double h, double S, double V, out int r, out int g, out int b)
        {
            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }
            r = Clamp((int)(R * 255.0));
            g = Clamp((int)(G * 255.0));
            b = Clamp((int)(B * 255.0));
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        static int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }
    }
}
