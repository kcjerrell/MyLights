using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace MyLights.Views
{
    /// <summary>
    /// Interaction logic for ColorPickerPopup.xaml
    /// </summary>
    public partial class ColorPickerPopup : Popup
    {
        public ColorPickerPopup()
        {
            InitializeComponent();
        }

        Task<Color> getColorTask;
        Color finalColor;
        Color initialColor;
        EventWaitHandle handle;

        public async Task<Color> GetColor(Color color = default, FrameworkElement placementTarget = null)
        {
            if (getColorTask != null && !getColorTask.IsCompleted)
            {
                throw new Exception("figure out what to do if there are two requests later. probably the answer is not use static");
                //also make sure you are initialized on the right thread derp
            }

            finalColor = color;
            initialColor = color;

            if (placementTarget != null)
                this.PlacementTarget = placementTarget;
            this.IsOpen = true;
            CaptureMouse();

            handle = new EventWaitHandle(false, EventResetMode.ManualReset);

            getColorTask = new Task<Color>(() =>
             {
                 handle.WaitOne();
                 return finalColor;
             });

            getColorTask.Start();

            return await getColorTask;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            finalColor = colorPicker.SelectedColor;
            IsOpen = false;
            handle.Set();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured && !IsMouseOver)
            {
                finalColor = initialColor;
                ReleaseMouseCapture();
                IsOpen = false;
                handle.Set();
            }
        }
    }
}
