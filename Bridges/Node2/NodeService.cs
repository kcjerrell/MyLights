using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using MyLights.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyLights.Bridges.Node2
{
    public class NodeService
    {
        const string INTEROP_MODULE = "C:/Users/kcjer/my-lights/light-rest/dist/interop.js";

        public NodeService()
        {
            var services = new ServiceCollection();
            services.AddNodeJS();

            //services.Configure<NodeJSProcessOptions>(options => options.ProjectPath = "C:\\Users\\kcjer\\my-lights\\light-rest\\src\\dist");
            services.Configure<NodeJSProcessOptions>(options => options.NodeAndV8Options = "--inspect-brk");
            services.Configure<OutOfProcessNodeJSServiceOptions>(options => options.TimeoutMS = -1);

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            INodeJSService nodeJSService = serviceProvider.GetRequiredService<INodeJSService>();

            this.node = nodeJSService;
        }

        INodeJSService node;
        Thread listenThread;


        private bool _isListening;

        public bool IsListening
        {
            get { return _isListening; }
            private set { _isListening = value; }
        }

        public async Task Connect()
        {
            await node.InvokeFromFileAsync<string>(INTEROP_MODULE, "init");
            Listen();
        }

        private void Listen()
        {
            listenThread = new Thread(async () =>
            {
                Trace.WriteLine("starting listener...");

                //throw new Exception("OMG THIS ONE IS BAD");

                IsListening = true;
                //RequestUpdateAll();

                while (IsListening)
                {
                    var message = await node.InvokeFromFileAsync<string>(INTEROP_MODULE, "listen");
                    ProcessMessage(message);
                }
            });

            listenThread.Start();
        }

        private void ProcessMessage(string message)
        {
            Trace.WriteLine(message);
            App.Current.Dispatcher.Invoke(() => RaiseMessageReceieved(message));
        }

        private void RaiseMessageReceieved(string message)
        {
            var handler = MessageReceived;
            handler?.Invoke(this, new MessageReceivedEventArgs(message));
        }

        public async Task SendMessage(string verb, string resource, string data = null)
        {
            string message;

            if (data != null)
                message = $"{verb}/{resource}/{data}";
            else
                message = $"{verb}/{resource}";

            Trace.WriteLine($"sending message: {message}");
            await SendMessage(message);
        }

        public async Task SendMessage(string message)
        {
           await node.InvokeFromFileAsync(INTEROP_MODULE, "send", new Object[] { message });
        }

        public event MessageReceivedEventHandler MessageReceived;

    }
}
