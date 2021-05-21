using MyLights.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Bridges.Udp
{
    public partial class UdpLightBridge
    {
        public class Client : IDisposable
        {
            //I this class should be limitted in its awareness of the messages it's sending/receiving. 
            //LightBridge can be aware of that (or an intermediate class)
            //But I think something like "UpdateAll" and building a list of devices should belong
            //somewhere else. This just connects, sends and receives messages.
            //At some point I should rename LightMessage/LightDgram, because even within the scope of
            //all this, I might have a switch/plug/sensor* at some point. (although sensors aren't
            //supported by tuyapi
            public Client(string address, int port, int listeningPort = DefaultPort)
            {
                Address = address;
                Port = port;
                ListeningPort = listeningPort;
            }

            public const int DefaultPort = 11000;

            System.Net.Sockets.UdpClient udp;
            Task listenTask;
            Logger log = new Logger("LightUdpClient");

            public string Address { get; }
            public int Port { get; }
            public int ListeningPort { get; }
            public bool IsConnected { get; private set; }
            public bool IsListening { get; private set; }

            public event LightMessageReceivedEventHandler LightMessageReceived;

            public async Task Connect()
            {
                log.Log("connecting...");
                udp = new System.Net.Sockets.UdpClient(ListeningPort);
                udp.Connect(Address, Port);

                log.Log("connection established");
                IsConnected = true;

                log.Log("saying hello");
                var msg = LightDgram.MakeHoller();
                await SendMessage(msg);

                log.Log("awaiting response...");
                var res = await udp.ReceiveAsync();

                log.Log("response received");
                listenTask = Task.Run(() => Listen());
            }

            private async Task Listen()
            {
                log.Log("starting listener...");

                IsListening = true;
                //RequestUpdateAll();

                while (IsListening)
                {
                    var rawMessage = await udp.ReceiveAsync();
                    ProcessMessage(rawMessage);
                }
            }

            int hitCount = 0;
            public async Task<int> SendMessage(LightDgram dgram)
            {
                hitCount += 1;
                if (hitCount % 10 == 0)
                    Trace.WriteLine(hitCount);

                Trace.WriteLine($"sending message: {dgram.ToString()}");
                //log.Log($"sending message: {dgram.ToString()}");
                dgram.GetBuffer(out byte[] bytes, out int length);
                return await udp.SendAsync(bytes, length);
            }

            public async Task<int> SendMessage(string msg)
            {
                //log.Log($"sending message: {msg}");
                Trace.WriteLine($"sending message: {msg}");
                var buffer = Encoding.UTF8.GetBytes(msg);
                return await udp.SendAsync(buffer, buffer.Length);
            }

            private void ProcessMessage(UdpReceiveResult rawMessage)
            {
                var dgram = LightDgram.FromBytes(rawMessage.Buffer);
                //log.Log($"message received: {dgram}");

                RaiseMessageReceived(dgram);
            }

            private void RaiseMessageReceived(LightDgram dgram)
            {
                var handler = LightMessageReceived;
                handler?.Invoke(this, new LightMessageEventArgs(dgram));
            }

            public void Dispose()
            {
                if (listenTask != null)
                {
                    listenTask.Dispose();
                }

                if (udp != null)
                {
                    udp.Dispose();
                }
            }
        }
    }
}