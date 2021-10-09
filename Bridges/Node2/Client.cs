using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyLights.Bridges.Node2
{
    public class Client : IDisposable
    {
        public Client(string address, int port, int listeningPort = DefaultPort)
        {
            Address = address;
            Port = port;
            ListeningPort = listeningPort;
        }

        public const int DefaultPort = 11000;

        System.Net.Sockets.UdpClient udp;
        Thread listenThread;

        public string Address { get; }
        public int Port { get; }
        public int ListeningPort { get; }
        public bool IsConnected { get; private set; }

        private volatile bool isListening;
        public bool IsListening { get => isListening; private set => isListening = value; }

        public event MessageReceivedEventHandler MessageReceived;

        public async Task Connect()
        {
            Trace.WriteLine("connecting...");
            udp = new System.Net.Sockets.UdpClient(ListeningPort);
            udp.Connect(Address, Port);

            Trace.WriteLine("connection established");
            IsConnected = true;

            await SendMessage("ping");
            await udp.ReceiveAsync();

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
                    var rawMessage = await udp.ReceiveAsync();
                    // Trace.WriteLine(rawMessage);
                    ProcessMessage(rawMessage);
                }
            });

            listenThread.Start();
        }

        // the only messages being sent here are
        // get/bulbs
        // get/bulb-n
        // set/bulb-n|json
        public async Task<int> SendMessage(string verb, string resource, string data = null)
        {
            string message;

            if (data != null)
                message = $"{verb}/{resource}/{data}";
            else
                message = $"{verb}/{resource}";

            Trace.WriteLine($"sending message: {message}");
            return await SendMessage(message);
        }

        public async Task<int> SendMessage(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);

            return await udp.SendAsync(buffer, buffer.Length);
        }

        // the only messages being receieved should be json objects
        private void ProcessMessage(UdpReceiveResult rawMessage)
        {
            var message = Encoding.UTF8.GetString(rawMessage.Buffer);
            Trace.WriteLine($"message received: {message}");

            App.Current.Dispatcher.Invoke(() => { RaiseMessageReceived(message); });
        }

        private void RaiseMessageReceived(string message)
        {
            var handler = MessageReceived;
            handler?.Invoke(this, new MessageReceivedEventArgs(message));
        }

        public void Dispose()
        {
            if (listenThread != null)
            {
                isListening = false;
                udp.Close();
                listenThread.Join();
            }

            if (udp != null)
            {
                udp.Dispose();
            }
        }
    }

    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(string message)
        {
            this.Message = message;
        }

        public string Message { get; }
    }
}
