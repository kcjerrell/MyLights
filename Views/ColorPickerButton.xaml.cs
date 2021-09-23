using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyLights.Views
{
    /// <summary>
    /// Interaction logic for ColorPickerButton.xaml
    /// </summary>
    public partial class ColorPickerButton : UserControl
    {
        public ColorPickerButton()
        {
            InitializeComponent();
        }     

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorPickerButton),
                new PropertyMetadata(Colors.White));

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Color = await Popup.GetColor(this.Color, this);
        }


        private static ColorPickerPopup _popup;
        private static ColorPickerPopup Popup
        {
            get
            {
                if (_popup == null) { _popup = new ColorPickerPopup(); }
                return _popup;
            }
        }

    }
}
