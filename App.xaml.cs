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
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            lightREST = new LightREST();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            lightREST.Dispose();
            base.OnExit(e);
        }

        LightREST lightREST;

        public Locator Locator { get => (Locator)this.Resources["Locator"]; }

        public static new App Current { get; private set; }
    }

}
