﻿using MyLights.LightMods;
using MyLights.Models;
using MyLights.Util;
using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyLights.Controls
{
    /// <summary>
    /// Interaction logic for LightControl.xaml
    /// </summary>
    public partial class LightControl : UserControl
    {
        public LightControl()
        {
            InitializeComponent();

            Loaded += LightControl_Loaded;
            DataContextChanged += LightControl_DataContextChanged;
        }

        private LightViewModel viewModel;

        private void LightControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is LightViewModel newLvm)
                viewModel = newLvm;
        }

        private LightViewModel GetDataContext()
        {
            return (LightViewModel)this.DataContext;
        }

        private void LightControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Locator.IsInDesignMode)
            {
                this.DataContext = Locator.Get.DesignLightVM;
            }
        }

        private void addColorToFavsButton_Click(object sender, RoutedEventArgs e)
        {
            Locator.Get.Library.AddColor(new Models.HSV(xySelector.ValueX, xySelector.ValueY, 1));
        }

        private void dmButtonFavColor_Checked(object sender, RoutedEventArgs e)
        {
            DisplayMode = LightControlDisplayMode.FavColors;
        }

        private void dmButtonFavColor_Unchecked(object sender, RoutedEventArgs e)
        {
            DisplayMode = LightControlDisplayMode.LightModeProperties;
        }

        private void ListBox_KeyDown(object sender, KeyEventArgs e)
        {
            var lb = (ListBox)sender;
            if (e.Key == Key.Delete)
            {
                if (lb.SelectedItem is HSV color)
                {
                    Locator.Get.Library.RemoveColor(color);
                }
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lb = (ListBox)sender;
            var color = (HSV)lb.SelectedItem;

            if (viewModel != null)
                viewModel.Color = color;
        }

        public LightControlDisplayMode DisplayMode
        {
            get { return (LightControlDisplayMode)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode", typeof(LightControlDisplayMode), typeof(LightControl),
                new PropertyMetadata(LightControlDisplayMode.LightModeProperties, (s, e) => ((LightControl)s).OnDisplayModeChanged(e)));

        private LightControlDisplayMode prevDispMode;
        private void OnDisplayModeChanged(DependencyPropertyChangedEventArgs e)
        {
            prevDispMode = (LightControlDisplayMode)e.OldValue;
            var mode = (LightControlDisplayMode)e.NewValue;

            dispModeLightModeContainer.Visibility = Visibility.Collapsed;
            dispModeFavColorContainer.Visibility = Visibility.Collapsed;
            dispModeSpecialContainer.Visibility = Visibility.Collapsed;

            switch (mode)
            {
                case LightControlDisplayMode.Minimized:
                    break;
                case LightControlDisplayMode.LightModeProperties:
                    dispModeLightModeContainer.Visibility = Visibility.Visible;
                    break;
                case LightControlDisplayMode.FavColors:
                    dispModeFavColorContainer.Visibility = Visibility.Visible;
                    break;
                case LightControlDisplayMode.Special:
                    dispModeSpecialContainer.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void dmButtonSpecial_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayMode == LightControlDisplayMode.Special)
                DisplayMode = prevDispMode;
            else
                DisplayMode = LightControlDisplayMode.Special;
        }

        private Dictionary<LightEffectsInfo, ILightEffect> effects = new();
        private void LightModButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is LightEffectsInfo lp)
            {
                if (effects.ContainsKey(lp))
                {
                    if (effects[lp].IsActive)
                        effects[lp].Suspend();
                    else
                        effects[lp].Start();
                }
                else
                {
                    effects[lp] = lp.Load();
                    effects[lp].Attach(Locator.Get.ModHost, new List<LightViewModel>() { viewModel });
                    effects[lp].Start();
                }
            }
        }

        private void lightModeListBox_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DisplayMode != LightControlDisplayMode.LightModeProperties)
                DisplayMode = LightControlDisplayMode.LightModeProperties;
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (viewModel != null)
                viewModel.IsLinked = !viewModel.IsLinked;
        }

        private void sceneRoot_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Scene.Stops.Add(new SceneStop());
        }

        private void SceneStop_DeleteRequested(object sender, EventArgs e)
        {
            var ssv = (MyLights.Views.SceneStop)sender;
            var ss = (SceneStop)ssv.DataContext;
            viewModel.Scene.Stops.Remove(ss);
        }
    }

    public enum LightControlDisplayMode
    {
        Minimized,
        LightModeProperties,
        FavColors,
        Special
    }
}
