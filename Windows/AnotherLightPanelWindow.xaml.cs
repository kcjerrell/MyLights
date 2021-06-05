using MyLights.LightMods;
using MyLights.Models;
using MyLights.Util;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

        private readonly Dictionary<LightEffectsInfo, ILightEffect> effects = new();

        private void GlobalMod_Click(object sender, RoutedEventArgs e)
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
                    effects[lp].Attach(Locator.Get.ModHost, Locator.Get.LightVMs.ToList());
                    effects[lp].Start();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var sequenceWindow = new SequenceEditor();
            sequenceWindow.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var lb = (Bridges.Udp.UdpLightBridge)Locator.Get.LightBridge;
            lb.Reload();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = (ListBox)sender;
            var selectedInfo = (LightEffectsInfo)listBox.SelectedItem;

            if (selectedInfo != null)
            {
                if (!effects.ContainsKey(selectedInfo))
                {
                    var effect = selectedInfo.Load();
                    effects[selectedInfo] = effect;
                }

                if (this.DataContext is ViewModels.AnotherLightPanelViewModel vm)
                    vm.SelectedMultiLightEffect = effects[selectedInfo];
            }
        }
    }
}
