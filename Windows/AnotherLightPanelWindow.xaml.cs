using MyLights.LightMods;
using MyLights.Models;
using MyLights.Util;
using MyLights.Windows.ViewModels;
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
using System.Windows.Shapes;

namespace MyLights.Windows
{
    /// <summary>
    /// Interaction logic for AnotherLightPanelWindow.xaml
    /// </summary>
    public partial class AnotherLightPanelWindow : Window
    {
        public AnotherLightPanelWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Logger.Log("click");
            var vm = (AnotherLightPanelViewModel)DataContext;
            vm.LightVMs.Add(App.Current.Locator.DesignLightVM);
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject item = (DependencyObject)sender;
            TextBlock label;
            TextBox input;

            do
            {
                item = VisualTreeHelper.GetChild(item, 0);
                if (item is ContentPresenter cp)
                {
                    label = (TextBlock)cp.ContentTemplate.FindName("sceneItemLabel", cp);
                    input = (TextBox)cp.ContentTemplate.FindName("sceneItemInput", cp);

                    label.Visibility = Visibility.Collapsed;
                    input.Visibility = Visibility.Visible;

                    input.LostFocus += Input_LostFocus;
                    input.KeyDown += Input_KeyDown;

                    editingSceneLabel = label;
                    editingSceneInput = input;

                    break;
                }
            }
            while (item != null);
        }

        private TextBlock editingSceneLabel;
        private TextBox editingSceneInput;

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EndEditing();
            }
        }

        private void Input_LostFocus(object sender, RoutedEventArgs e)
        {
            EndEditing();
        }

        private void EndEditing()
        {
            editingSceneLabel.Visibility = Visibility.Visible;
            editingSceneInput.Visibility = Visibility.Collapsed;

            editingSceneInput.KeyDown -= Input_KeyDown;
            editingSceneInput.LostFocus -= Input_LostFocus;

            Locator.Get.Library.SaveLibrary();
        }

        private void scenesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count >= 1)
            {
                var scene = (Scene)e.AddedItems[0];
                Locator.Get.Library.ApplyScene(scene);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var scene = (Scene)button.DataContext;

            Locator.Get.Library.RemoveScene(scene);
        }

        private Dictionary<ILightPlugin, IGlobalMod> globalMods = new();
        private void GlobalMod_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ILightPlugin lp)
            {
                if (globalMods.ContainsKey(lp))
                {
                    if (globalMods[lp].IsActive)
                        globalMods[lp].Suspend();
                    else
                        globalMods[lp].Start();
                }
                else
                {
                    globalMods[lp] = lp.GetGlobalMod(Locator.Get.ModHost);
                    globalMods[lp].Start();
                }
            }
        }
    }
}
