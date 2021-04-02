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

        SolidColorBrush selectedColorBrush = new SolidColorBrush();

        private void ColorPicker_Loaded(object sender, RoutedEventArgs e)
        {
            selectedColorBorder.Background = selectedColorBrush;

            //shadeBrush = new SolidColorBrush();
            //colorThumb.Foreground = shadeBrush;
            //
            //vMin = new GradientStop(Colors.Black, 1);
            //vMax = new GradientStop(Colors.White, 0);
            //var stops = new GradientStopCollection() { vMin, vMax };
            //vBrush = new LinearGradientBrush(stops);
            //valueBorder.Background = vBrush;

        }

        //private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        //{
        //    double x = colorThumb.Margin.Left + e.HorizontalChange;
        //    x = x.Clamp(0.0, spectrumBorder.ActualWidth);
        //
        //    double y = colorThumb.Margin.Top + e.VerticalChange;
        //    y = y.Clamp(0.0, spectrumBorder.ActualHeight);
        //
        //    colorThumb.Margin = new Thickness(x, y, 0, 0);
        //
        //    SelectedShade = spectrum.GetColor(x, y, 1);
        //
        //    //shadeBrush.Color = spectrum.GetColor(x, y, 1);
        //
        //    //vBrush.GradientStops[1] = new GradientStop(shadeBrush.Color,0);
        //    //
        //    //SelectedColor = shadeBrush.Color;
        //}


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
            selectedColorBrush.Color = SelectedColor;
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
            //if (DataContext is ColorPickerViewModel vm)
            //{
            //    vm.SelectedShade = (Color)e.NewValue;
            //}

            UpdateColor();
        }

        private void UpdateColor()
        {
            if (ShowValueSlider)
                SelectedColor = Helpers.HsvToColor(xySelector.ValueX, xySelector.ValueY, valueSlider.Value);
            else
                SelectedColor = SelectedShade;


        }

        //private void XYSelector_ValueChanged(object sender, Controls.PointValueChangedEventArgs e)
        //{
        //    SelectedShade = Helpers.HsvToColor(e.Value.X, e.Value.Y, 1.0);
        //    Hue = e.Value.X;
        //    Saturation = e.Value.Y;
        //}

        private void valueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateColor();
        }

        public bool ShowValueSlider
        {
            get { return (bool)GetValue(ShowValueSliderProperty); }
            set { SetValue(ShowValueSliderProperty, value); }
        }

        public static readonly DependencyProperty ShowValueSliderProperty =
            DependencyProperty.Register("ShowValueSlider", typeof(bool), typeof(ColorPicker),
                new PropertyMetadata(true, (s, e) => ((ColorPicker)s).OnShowValueSliderChanged(e)));

        private void OnShowValueSliderChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool show)
            {
                if (show)
                    valueBorder.Visibility = Visibility.Visible;
                else
                    valueBorder.Visibility = Visibility.Collapsed;
            }
        }

        public double Hue
        {
            get { return (double)GetValue(HueProperty); }
            set { SetValue(HueProperty, value); }
        }

        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register("Hue", typeof(double), typeof(ColorPicker),
                new PropertyMetadata(0.0, (s, e) => ((ColorPicker)s).OnHueChanged(e)));

        private void OnHueChanged(DependencyPropertyChangedEventArgs e)
        {
            double oldValue = (double)e.OldValue;
            double newValue = (double)e.NewValue;

            if (oldValue != newValue)
            {
                xySelector.ValueX = Hue;

            }
        }

        public double Saturation
        {
            get { return (double)GetValue(SaturationProperty); }
            set { SetValue(SaturationProperty, value); }
        }

        public static readonly DependencyProperty SaturationProperty =
            DependencyProperty.Register("Saturation", typeof(double), typeof(ColorPicker),
                new PropertyMetadata(0.0, (s, e) => ((ColorPicker)s).OnSaturationChanged(e)));

        private void OnSaturationChanged(DependencyPropertyChangedEventArgs e)
        {
            double oldValue = (double)e.OldValue;
            double newValue = (double)e.NewValue;

            if (oldValue != newValue)
            {
                xySelector.ValueY = Saturation;
            }
        }
    }
}
