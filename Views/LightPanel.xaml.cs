using MyLights.Util;
using MyLights.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyLights.Views
{
    /// <summary>
    /// Interaction logic for LightTrack.xaml
    /// </summary>
    public partial class LightPanel : UserControl
    {
        public LightPanel()
        {
            InitializeComponent();
            AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(SomeMouseLeftButtonUp), true);
            Loaded += LightPanel_Loaded;
        }

        private async void LightPanel_Loaded(object sender, RoutedEventArgs e)
        {
            //loading.Show();
            await Task.Delay(1500);
            await LightBridge.Singleton.GetLights();
            //loading.Hide();
        }

        private void FlyoutClosedCallBack()
        {
            // #lightgroup 
            lightGroup?.Ungroup();
        }

        private void SomeMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseDragging)
            {
                mouseDragging = false;

                var args = new FlyoutRequestEventArgs();

                var selected = (from LightViewModel lvm in itemsControl.Items
                                where lvm.IsSelected
                                select lvm).ToList();

                if (selected.Count == 0)
                    return;

                // #lightgroup 
                else if (selected.Count > 1)
                    lightGroup = new LightGroupViewModel(selected);

                args.Source = selected[0];
                args.FlyoutClosedCallBack = FlyoutClosedCallBack;

                var container = (FrameworkElement)itemsControl.ItemContainerGenerator.ContainerFromItem(selected.First());
                args.FlyoutPosition = container.TranslatePoint(new Point(container.ActualWidth, 0.0), null);

                var handler = FlyoutRequest;
                handler?.Invoke(this, args);
            }
        }

        private bool mouseDragging = false;
        private bool selecting = false;
        // #lightgroup 
        private LightGroupViewModel lightGroup;

        private void LightPanelItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = (LightPanelItem)sender;
            var vm = (LightViewModel)item.DataContext;
            selecting = !vm.IsSelected;
            vm.IsSelected = selecting;

            mouseDragging = true;
        }

        public event FlyoutRequestEventHandler FlyoutRequest;

        private void LightPanelItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (mouseDragging)
            {
                var item = (LightPanelItem)sender;
                var vm = (LightViewModel)item.DataContext;
                vm.IsSelected = selecting;
            }
        }

        private void LightPanelItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (LightPanelItem)sender;
            var vm = (LightViewModel)item.DataContext;

            foreach (LightViewModel lvm in itemsControl.Items)
            {
                lvm.IsSelected = false;
            }

            vm.IsSelected = true;
        }
    }

    public delegate void FlyoutRequestEventHandler(object sender, FlyoutRequestEventArgs e);

    public class FlyoutRequestEventArgs : EventArgs
    {
        public object Source { get; set; }
        public Point FlyoutPosition { get; set; }

        public Action FlyoutClosedCallBack { get; set; }
    }
}
