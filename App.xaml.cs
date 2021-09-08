using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using MyLights.Util;
using MyLights.Windows;
using MyLights.Models;

namespace MyLights
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Startup += App_Startup;            
        }

        private async void App_Startup(object sender, StartupEventArgs e)
        {
            // await Locator.StartServices();
        }

        private DevConsole devConsole;

        public Locator Locator = new Locator();

        public static new App Current
        {
            get
            {
                if (Application.Current is App current)
                    return current;
                else
                    return null;
            }
        }
    }

}
