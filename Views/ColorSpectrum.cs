using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyLights.Views
{
    public class ColorSpectrum : FrameworkElement
    {

        WriteableBitmap bitmap;

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawImage(bitmap, new Rect(0, 0, RenderSize.Width, RenderSize.Height));
            drawingContext.Close();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            bitmap = new WriteableBitmap((int)sizeInfo.NewSize.Width, (int)sizeInfo.NewSize.Height, 96, 96, PixelFormats.Bgr32, null);
            GenerateSpectrum(bitmap);
        }

        private void GenerateSpectrum(WriteableBitmap bitmap)
        {
            var data = new byte[bitmap.PixelWidth * bitmap.PixelHeight * 3];

            int r, g, b;
            double x, y, h, s, v;

            v = 1;

            for (int i = 0; i < data.Length; i += 3)
            {
                x = i % bitmap.PixelWidth;
                y = i / bitmap.PixelWidth;

                h = x / bitmap.PixelWidth;
                s = y / bitmap.PixelHeight;

                HsvToRgb(h, s, v, out r, out g, out b);

                data[i] = (byte)r;
                data[i + 1] = (byte)g;
                data[i + 2] = (byte)b;
            }

            bitmap.Lock();
            bitmap.WritePixels(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), data, 3 * bitmap.PixelWidth, 0, 0);
            bitmap.Unlock();
        }

        void HsvToRgb(double h, double S, double V, out int r, out int g, out int b)
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
        int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }
    }
}
