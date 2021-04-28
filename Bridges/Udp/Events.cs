﻿using MyLights.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Bridges.Udp
{
    public delegate void LightMessageReceivedEventHandler(object sender, LightMessageEventArgs e);

    public delegate void OutgoingChangeRequestedEventHandler(UdpProperty sender, OutgoingChangeRequestedEventArgs e);

    public class LightMessageEventArgs
    {
        public LightMessageEventArgs(LightDgram message)
        {
            Message = message;
        }

        public LightDgram Message { get; set; }
    }

    public class OutgoingChangeRequestedEventArgs
    {
        public OutgoingChangeRequestedEventArgs()
        {
        }
        public static OutgoingChangeRequestedEventArgs Default { get; } = new OutgoingChangeRequestedEventArgs();
    }
}
