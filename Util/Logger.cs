using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

        public void Log(string message, int priority = 3)
        {
            Logger.Log(CallerInformation, message, priority);
        }

        static Logger()
        {            
            uiThreadId = App.Current.MainWindow.Dispatcher.Thread.ManagedThreadId;
        }

        public static int LogToConsolePriority { get; set; } = 3;

        // 1: overkill messages (ie: logging every udp message)
        // 2: messages worth logging, but don't need to be seen at runtime
        // 3: default priority for sending messages directly to the console/output
        // 4: Important
        // 5: Critical

        private static int uiThreadId;

        public static void Log(string message, string callerInfo = "", int priority = 3)
        {
            if (priority >= LogToConsolePriority)
            {
                int currentThread = Thread.CurrentThread.ManagedThreadId;
                string onUi = currentThread == uiThreadId ? "*" : "";
                string output = $"{callerInfo}: {message} [{currentThread}{onUi}]";

                if (priority == 5)
                    Trace.TraceError(output);
                else if (priority == 4)
                    Trace.TraceWarning(output);
                else
                    Trace.TraceInformation(output);
            }
            else
            {
                // disregard... lol
            }
        }
    }
}
