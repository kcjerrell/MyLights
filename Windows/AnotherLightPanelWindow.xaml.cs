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
