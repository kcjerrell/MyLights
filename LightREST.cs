using MyLights.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights
{
    public class LightREST
    {
        private async static Task StartServer()
        {
            if (Locator.IsInDesignMode)
                return;

            var startInfo = new ProcessStartInfo("node");
            startInfo.ArgumentList.Add("--trace-warnings");
            startInfo.ArgumentList.Add("--unhandled-rejections=warn");
            startInfo.ArgumentList.Add("--trace-uncaught");
            startInfo.ArgumentList.Add(@"C:\Users\kcjer\source\repos\lightrest\dist\server.js");
            var node = Process.Start(startInfo);
            await Task.Delay(1000);
            isServerStarted = true;
        }

        static bool isServerStarted = false;

        internal async static Task<bool> CheckServer()
        {
            if (Locator.IsInDesignMode)
                return true;

            if (!isServerStarted)
                await StartServer();

            return isServerStarted;
        }
    }
}
