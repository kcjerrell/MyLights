﻿using MyLights.Util;
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
    }
}
