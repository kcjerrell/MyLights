using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using MyLights.Util;

namespace MyLights
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Current = this;
            this.Startup += App_Startup;
            this.Exit += App_Exit;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            var startInfo = new ProcessStartInfo("node");
            startInfo.ArgumentList.Add("--trace-warnings");
            startInfo.ArgumentList.Add("--unhandled-rejections=warn");
            startInfo.ArgumentList.Add("--trace-uncaught");
            startInfo.ArgumentList.Add(@"C:\Users\kcjer\source\repos\lightrest\dist\server.js");
            node = Process.Start(startInfo);
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            if (node != null)
            {
                node.Kill();
            }
        }

        private Process node;

        public Locator Locator { get => (Locator)this.Resources["Locator"]; }

        public static new App Current { get; private set; }
    }

}
