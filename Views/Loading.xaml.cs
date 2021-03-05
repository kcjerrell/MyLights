﻿using System;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyLights.Views
{
    /// <summary>
    /// Interaction logic for Loading.xaml
    /// </summary>
    public partial class Loading : UserControl
    {
        public Loading()
        {
            InitializeComponent();
        }

        public void Show()
        {
            var loadingAnim = (Storyboard)Resources["LoadingAnim"];
            this.Visibility = Visibility.Visible;
            loadingAnim.Begin();
        }

        public void Hide()
        {
            var loadingAnim = (Storyboard)Resources["LoadingAnim"];
            loadingAnim.Stop();
            this.Visibility = Visibility.Collapsed;
        }


    }
}
