using MyLights.Util;
using MyLights.ViewModels;
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

namespace MyLights.Views
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        public ColorPicker()
        {
            InitializeComponent();
            Loaded += ColorPicker_Loaded;
        }

        private void ColorPicker_Loaded(object sender, RoutedEventArgs e)
        {
            //shadeBrush = new SolidColorBrush();
            //colorThumb.Foreground = shadeBrush;
            //
            //vMin = new GradientStop(Colors.Black, 1);
            //vMax = new GradientStop(Colors.White, 0);
            //var stops = new GradientStopCollection() { vMin, vMax };
            //vBrush = new LinearGradientBrush(stops);
            //valueBorder.Background = vBrush;

        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double x = colorThumb.Margin.Left + e.HorizontalChange;
            x = x.Clamp(0.0, spectrumBorder.ActualWidth);

            double y = colorThumb.Margin.Top + e.VerticalChange;
            y = y.Clamp(0.0, spectrumBorder.ActualHeight);

            colorThumb.Margin = new Thickness(x, y, 0, 0);

            SelectedShade = spectrum.GetColor(x, y, 1);

            //shadeBrush.Color = spectrum.GetColor(x, y, 1);

            //vBrush.GradientStops[1] = new GradientStop(shadeBrush.Color,0);
            //
            //SelectedColor = shadeBrush.Color;
        }


        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker),
                new PropertyMetadata(default(Color), (s, e) => ((ColorPicker)s).OnSelectedColorChanged(e)));

        private void OnSelectedColorChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        public Color SelectedShade
        {
            get { return (Color)GetValue(SelectedShadeProperty); }
            set { SetValue(SelectedShadeProperty, value); }
        }

        public static readonly DependencyProperty SelectedShadeProperty =
            DependencyProperty.Register("SelectedShade", typeof(Color), typeof(ColorPicker),
                new PropertyMetadata(default(Color), (s, e) => ((ColorPicker)s).OnSelectedShadeChanged(e)));

        private void OnSelectedShadeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is ColorPickerViewModel vm)
            {
                vm.SelectedShade = (Color)e.NewValue;
            }
        }


    }
}
