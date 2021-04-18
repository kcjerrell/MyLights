using MyLights.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.LightUdp
{

    // verb:target:property:data
    // get:bulb-1:power:
    // set:bulb-3:color:1.0-1.0-1.0
    // greet:main:none:
    // getall:bulb-*:none:

    // oh wait I forgot that udp doesn't follow the request-response pattern
    // meaning that while something like "list:bulb-*:id:" might make sense as a "request" that epects a "response"
    // really it's saying "hey, I'd like to know all the bulb id's you got. If you get around to it..."
    // which is fine, but as it is right now, how do I encode the "response"?
    // 
    // Get isn't really Get as well. It's more like "Wonder". I think I might make that the verb to use.
    // client:  "wonder:bulb-1:power:"
    // server:  "

    public struct LightDgram
    {
        public LightDgram(DgramVerbs verb = DgramVerbs.None, string target = "none", LightProperties property = LightProperties.None, string data = "")
        {
            Verb = verb;
            Target = target;
            Property = property;
            _data = data;
        }

        public DgramVerbs Verb { get; set; }
        public string Target { get; set; }
        public LightProperties Property { get; set; }

        private string _data;
        public string Data
        {
            get
            {
                if (string.IsNullOrEmpty(_data))
                    return string.Empty;
                else
                    return _data;
            }
            set
            {
                _data = value;
            }
        }

        public string GetFormatted()
        {
            string msg =
                $"{Verb.ToString().ToLower()}:" +
                $"{Target}:" +
                $"{Property.ToString().ToLower()}:" +
                $"{Data}";

            return msg;
        }

        public byte[] ToBytes()
        {
            string msg = GetFormatted();
            return Encoding.UTF8.GetBytes(msg);
        }
        public void GetBuffer(out byte[] bytes, out int length)
        {
            bytes = this.ToBytes();
            length = bytes.Length;
        }

        public override string ToString()
        {
            // "(LightDgram: Get Bulb-1 Power )"
            // "(LightDgram: Set Bulb-2 Color 1.0-1.0-1.0)"
            return $"(LightDgram: {Verb} {Target} {Property.ToString()} {Data})";
        }

        public static LightDgram FromBytes(byte[] bytes)
        {
            string message = Encoding.UTF8.GetString(bytes);

            string[] segments = message.Split(':');

            return new LightDgram()
            {
                Verb = (DgramVerbs)Enum.Parse(typeof(DgramVerbs), segments[0], true),
                Target = segments[1],
                Property = (LightProperties)Enum.Parse(typeof(LightProperties), segments[2], true),
                Data = segments[3]
            };
        }

        public static LightDgram ForColor(int bulbId)
        {
            return ForColor(bulbId, false);
        }

        public static LightDgram ForColor(int bulbId, HSV color)
        {
            return ForColor(bulbId, true, color);
        }

        private static LightDgram ForColor(int bulbId, bool useColor, HSV color = default)
        {
            return new LightDgram()
            {
                //Verb = useColor ? DgramVerbs.Set : DgramVerbs.Get,
                Target = $"bulb-{bulbId}",
                Property = LightProperties.Color,
                Data = useColor ? $"{color.H.ToString("F3")}-{color.S.ToString("F3")}-{color.V.ToString("F3")})" : ""
            };
        }

        public static LightDgram ForPower(int bulbId, bool power)
        {
            return new LightDgram()
            {
               // Target = bulbId,
                Property = LightProperties.Power,
                Data = power.ToString()
            };
        }
    }
}
