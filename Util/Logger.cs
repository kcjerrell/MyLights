using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

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
            uiDispatcher = App.Current.Dispatcher;
            uiThreadId = uiDispatcher.Thread.ManagedThreadId;
            LogItems = new ReadOnlyObservableCollection<LogItem>(logItems);

            // Log("Here's a dummy message", "Logger",5);
            // Log("And another one", "Logger");
        }

        private static ObservableCollection<LogItem> logItems = new ObservableCollection<LogItem>();
        private static int uiThreadId;
        private static Dispatcher uiDispatcher;

        public static ReadOnlyObservableCollection<LogItem> LogItems { get; }
        public static int LogToConsolePriority { get; set; } = 3;
        public static bool IsTraceEnabled { get; set; } = false;

        // 1: overkill messages (ie: logging every udp message)
        // 2: messages worth logging, but don't need to be seen at runtime
        // 3: default priority for sending messages directly to the console/output
        // 4: Important
        // 5: Critical


        public static void Log(string message, string callerInfo = "", int priority = 3)
        {
            var item = new LogItem(message, callerInfo, priority);

            uiDispatcher?.Invoke(() => logItems.Add(item));

            //logItems.Add(item);

            if (IsTraceEnabled && priority >= LogToConsolePriority)
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

            }
        }

    }

    public struct LogItem
    {
        public LogItem(string message, string sender, int priority)
        {
            this.Message = message;
            this.Sender = sender;
            this.Priority = priority;

            this.Time = DateTime.Now;
            this.ThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public string Message { get; set; }
        public DateTime Time { get; set; }
        public int Priority { get; set; }
        public string Sender { get; set; }
        public int ThreadId { get; set; }
    }
}
