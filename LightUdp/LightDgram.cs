using MyLights.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.LightUdp
{
    public struct LightDgram
    {
        public LightDgram(DgramVerbs verb = DgramVerbs.None, string target = "", LightProperties property = LightProperties.None, string data = "")
        {
            Verb = verb;
            Target = target;
            Property = property;
            _data = data;
        }

        public DgramVerbs Verb { get; init; }
        public string Target { get; init; }
        public LightProperties Property { get; init; }

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
            init
            {
                _data = value;
            }
        }

        public override string ToString()
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
            string msg = ToString();
            return Encoding.UTF8.GetBytes(msg);
        }
        public void GetBuffer(out byte[] bytes, out int length)
        {
            bytes = this.ToBytes();
            length = bytes.Length;
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
    }
}
