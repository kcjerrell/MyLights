using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Util
{
    public class Logger
    {
        public Logger(string callerInformation)
        {
            CallerInformation = callerInformation;
        }

        public string CallerInformation { get; set; }

        public void Log(string message)
        {
            Logger.Log(CallerInformation, message);
        }

        public static void Log(string message, string callerInfo = "")
        {
            Console.WriteLine($"{callerInfo}: {message}");
        }
    }
}
