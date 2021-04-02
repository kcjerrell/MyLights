using MyLights.Util;
using MyLights.ViewModels;
using MyLights.Views;
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
using System.Windows.Shapes;

namespace MyLights
{
    /// <summary>
    /// Interaction logic for LightPaletteWindow.xaml
    /// </summary>
    public partial class LightPaletteWindow : Window
    {
        public LightPaletteWindow()
        {
            InitializeComponent();
        }

        List<Thumb> syncThumbs = new List<Thumb>();
        //Color[,] colorMap;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            this.Background = new SolidColorBrush(await ColorPickerPopup.GetColor(placementTarget: button));
        }

        private void Thumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            var thumb = (Thumb)sender;
            if (thumb.Parent != overlayCanvas)
            {
                var dc = thumb.DataContext;
                var offset = thumb.TranslatePoint(new Point(0, 0), overlayCanvas);
                Canvas.SetLeft(thumb, offset.X);
                Canvas.SetTop(thumb, offset.Y);
                var grid = (Grid)thumb.Parent;
                grid.Children.Remove(thumb);
                overlayCanvas.Children.Add(thumb);
                thumb.DataContext = dc;

                syncThumbs.Add(thumb);
            }

            //RenderTargetBitmap bmp = new RenderTargetBitmap((int)spectrum.ActualWidth, (int)spectrum.ActualHeight,
            //    96, 96, PixelFormats.Bgr32);
            //bmp.Render(spectrum);
            //
            //var data = new byte[bmp.PixelWidth * bmp.PixelHeight * 4];
            //bmp.CopyPixels(data, bmp.PixelWidth * 4, 0);
            //
            //colorMap = new Color[bmp.PixelWidth, bmp.PixelHeight];
            //
            //for (int i = 0; i < data.Length; i += 4)
            //{
            //    int x = i / 4 % bmp.PixelWidth;
            //    int y = i / 4 / bmp.PixelWidth;
            //
            //    colorMap[x, y] = Color.FromRgb(data[i + 2], data[i + 1], data[i]);
            //}
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

            var thumb = (Thumb)sender;

            var dragList = new List<Thumb>();

            if (syncCheckBox.IsChecked.HasValue && syncCheckBox.IsChecked.Value)
                dragList = syncThumbs;

            else
                dragList.Add(thumb);

            foreach (var t in dragList)
            {
                double x = Canvas.GetLeft(t);
                Canvas.SetLeft(t, x + e.HorizontalChange);

                double y = Canvas.GetTop(t);
                Canvas.SetTop(t, y + e.VerticalChange);

                Point cp = t.TranslatePoint(new Point(10, 10), spectrum);

                if (cp.X >= 0 && cp.Y >= 0)
                {
                    var color = spectrum.GetColor(cp.X, cp.Y, 1.0);
                    ((LightViewModel)thumb.DataContext).Color = color.ToHSV();
                }


            }
        }

        private void Thumb_DragOver(object sender, DragEventArgs e)
        {

        }
    }
}
