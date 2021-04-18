using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.LightUdp
{
    public delegate void LightMessageReceivedEventHandler(object sender, LightMessageEventArgs e);

    public class LightMessageEventArgs
    {
        public LightMessageEventArgs(LightDgram message)
        {
            Message = message;
        }

        public LightDgram Message { get; set; }
    }
}
