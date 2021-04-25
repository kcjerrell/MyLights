using MyLights.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Bridges.Udp
{
    /* Right now messages resemble:
     * 
     *      wish:bulb-1:power:false
     *      wonder:bulb-2:mode:
     *      tell:bulb-3:brightness:900
     *      
     *      I need to rework it so I can set multiple properties in one message. Since ':' is 
     *      defacto reserved right now, I think I'll just expand that
     *      
     *      wish:bulb-1:power::true:mode::white:brightness::500
     *      
     *      Actually I'll use another charactor, because I don't know if splitting on : vs :: is going to be annoying. 
     *      I'll just use =
     *      
     *      tell:bulb-2:mode=color:color=.9-1-1:power=true
     *      
     *      split on :
     *      [0] verb
     *      [2] resourceID
     *      [3...] property/value pairs
     *      
     *      Except I'm not sure how I should do wonder messages....
     *      
     *      wonder:bulb-1:name:mode:color:power:brightness:colortemp
     *      
     *              or
     *              
     *      wonder:bulb-1:name=?:mode=?:color=?:power yeah no the first one
     *      
     *      
     *      OKay so the new official Wish form of the dgram is.... drum roll please...
     *      
     *      wish:target:[prop=value(:)]
     *      
     *      wish:bulb-2:power=true:mode=color:color=.5-.5-.5
     */

    public struct LightDgram
    {
        public LightDgram(DgramVerbs verb = DgramVerbs.None, string target = "", DgramProperties property = DgramProperties.None, string data = "")
        {
            Verb = verb;
            Target = target;
            Property = property;
            _data = data;
        }

        public DgramVerbs Verb { get; init; }
        public string Target { get; init; }
        public DgramProperties Property { get; init; }

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
            bytes = ToBytes();
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
                Property = (DgramProperties)Enum.Parse(typeof(DgramProperties), segments[2], true),
                Data = segments[3]
            };
        }

        // la la la
        public static string MakeWish(string target, IEnumerable<string> propValues)
        {
            return $"{DgramVerbs.Wish.ToString().ToLower()}:{target}:{string.Join(':', propValues)}";

        }
    }
}
