using MyLights.Models;
using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static MyLights.Bridges.Udp.UdpLightBridge;

namespace MyLights.Bridges.Udp
{
    /*  For bundling/gating property changes, here's the pattern
     *  
     *  - Consumer calls Set() on a property
     *  - the property stores that value in OutgoingValue
     *      - if multiple changes are requested before processed, only the most recent
     *        request will be saved
     *  - the property sets a flag, HasPendingChange
     *  - the property raises the event ChangeRequested
     *  - Subscribed to that event is the PropertiesProvider
     *      - If it's already processing changes, the event is disccarded. otherwise,
     *        the properties provider starts its loop
     *      - after a brief delay (to gather multiple changes requested in a brief window),
     *        the PropertiesProvider iterates through it's properties for any that have
     *        HasPendingChange true
     *      - PP gathers the dgram formatted requests from these properties, resetting
     *        HasPendingCHhange.
     *      - It combines the requests into a single message and sends it
     *      - After a brief delay, the properties are checked again for HasPendingChanges
     *      - if there are any, the cycle repeats. If not, it ends
     *      
     *      - although right now I have:
     *          - OnUpdated()
     *          - UpdateValue()
     *          - Set()
     *          - RequestChange()
     *          - UPdate()
     *          
     *       lols
     * */

    public abstract class UdpProperty
    {
        private volatile bool _hasPendingChange;
        public bool HasPendingChange { get => _hasPendingChange; protected set => _hasPendingChange = value; }
        protected abstract LightProperties AssociatedProperty { get; }
        public abstract string GetOutgoingFragment(bool clearPendingStatus = false);
    }

    public abstract class UdpProperty<T> : UdpProperty, IDeviceProperty<T> where T : IEquatable<T>
    {

        private T _value;
        public T Value
        {
            get => _value;
            protected set
            {
                UpdateValue(value);
            }
        }

        public virtual void UpdateValue(T value)
        {
            if (!Compare(value, _value))
            {
                _value = value;

                var handler = Updated;
                handler?.Invoke(this, UpdateEventArgs);
            }
        }

        public event EventHandler OutgoingChangeRequested;
        public Task Set(T newValue)
        {
            OutgoingValue = newValue;
            HasPendingChange = true;
            //var handler = OutgoingChangeRequested;
            //handler?.Invoke(this, OutgoingChangeRequestedEventArgs);

            OutgoingChangeRequested.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        public async Task Update()
        {

        }

        public override string GetOutgoingFragment(bool clearOutgoingStatus = false)
        {
            if (clearOutgoingStatus)
                HasPendingChange = false;

            return GetWishFragment(OutgoingValue);
        }

        public event PropertyChangedEventHandler Updated;
        public abstract T OutgoingValue { get; protected set; }
        protected abstract PropertyChangedEventArgs UpdateEventArgs { get; }
        protected abstract string GetWishFragment(T value);
        protected abstract string GetWonderFragment();
        protected virtual bool Compare(T first, T second)
        {
            if (first == null)
                return false;
            return first.Equals(second);
        }
    }

    public class UdpPower : UdpProperty<bool>
    {
        private static PropertyChangedEventArgs updateEventArgs = new PropertyChangedEventArgs("Power");

        private volatile bool _outgoingValue;
        public override bool OutgoingValue
        {
            get => _outgoingValue;
            protected set => _outgoingValue = value;
        }

        protected override PropertyChangedEventArgs UpdateEventArgs => updateEventArgs;

        protected override string GetWishFragment(bool value)
        {
            return $"{LightProperties.Power.ToString().ToLower()}={value}";
        }

        protected override string GetWonderFragment()
        {
            return $"{LightProperties.Power.ToString().ToLower()}";
        }

        protected override LightProperties AssociatedProperty => LightProperties.Power;
    }

    public class UdpColor : UdpProperty<HSV>
    {
        object colorLocker = new object();
        HSV _outgoingValue;
        public override HSV OutgoingValue
        {
            get
            {
                lock (colorLocker)
                {
                    return _outgoingValue;
                }
            }
            protected set
            {
                lock (colorLocker)
                {
                    _outgoingValue = value;
                }
            }
        }

        private static PropertyChangedEventArgs updateEventArgs = new("Color");
        protected override PropertyChangedEventArgs UpdateEventArgs => updateEventArgs;

        private static Regex pattern = new Regex("h([0-9.]+)s([0-9.]+)v([0-9.]+)");
        internal static HSV DecodeColor(string data)
        {
            var match = pattern.Match(data).Groups;
            double h = double.Parse(match[1].ToString());
            double s = double.Parse(match[2].ToString());
            double v = double.Parse(match[3].ToString());

            return new HSV(h, s, v);
        }

        internal static string EncodeColor(HSV color)
        {
            return $"h{color.H}s{color.S}v{color.V}";
        }

        protected override bool Compare(HSV a, HSV b)
        {
            if (Math.Abs(a.H - b.H) > 0.01)
                return false;
            if (Math.Abs(a.S - b.S) > 0.01)
                return false;
            if (Math.Abs(a.V - b.V) > 0.01)
                return false;

            return true;
        }

        protected override string GetWishFragment(HSV value)
        {
            return $"{LightProperties.Color.ToString().ToLower()}={EncodeColor(value)}";
        }

        protected override string GetWonderFragment()
        {
            return $"{LightProperties.Color.ToString().ToLower()}";
        }

        protected override LightProperties AssociatedProperty => LightProperties.Color;

    }

    public class UdpMode : UdpProperty<string>
    {
        private volatile LightMode mode;
        public override string OutgoingValue
        {
            get
            {
                return ModeToString(mode);
            }
            protected set
            {
                mode = StringToMode(value);
            }
        }

        public override void UpdateValue(string value)
        {
            base.UpdateValue(value == "colour" ? "color" : value);
        }

        protected override string GetWishFragment(string value)
        {
            return $"{LightProperties.Mode.ToString().ToLower()}={value}";
        }

        protected override string GetWonderFragment()
        {
            return $"{LightProperties.Mode.ToString().ToLower()}";
        }

        public static string ModeToString(LightMode mode)
        {
            return mode switch
            {
                LightMode.None => "",
                LightMode.Color => "color",
                LightMode.White => "white",
                LightMode.Music => "music",
                _ => ""
            };
        }

        public static LightMode StringToMode(string mode)
        {
            return mode switch
            {
                "color" => LightMode.Color,
                "white" => LightMode.White,
                "music" => LightMode.Music,
                _ => LightMode.None
            };
        }

        private static PropertyChangedEventArgs updateEventArgs = new PropertyChangedEventArgs("Mode");
        protected override PropertyChangedEventArgs UpdateEventArgs => updateEventArgs;
        protected override LightProperties AssociatedProperty => LightProperties.Mode;

    }

    public class UdpBrightness : UdpProperty<double>
    {
        private object brightLock = new object();

        private double _outgoingValue;
        public override double OutgoingValue
        {
            get
            {
                lock (brightLock)
                {
                    return _outgoingValue;
                }
            }
            protected set
            {
                lock (brightLock)
                {
                    _outgoingValue = value;
                }
            }
        }
        protected override PropertyChangedEventArgs UpdateEventArgs => updateEventArgs;
        private static PropertyChangedEventArgs updateEventArgs = new PropertyChangedEventArgs("Brightness");

        protected override string GetWishFragment(double value)
        {
            return $"{LightProperties.Brightness.ToString().ToLower()}={value.ToString("F2")}";
        }

        protected override string GetWonderFragment()
        {
            return $"{LightProperties.Brightness.ToString().ToLower()}";
        }
        protected override LightProperties AssociatedProperty => LightProperties.Brightness;

    }

    public class UdpColorTemp : UdpProperty<double>
    {
        private object ctLock = new object();

        private double _outgoingValue;
        public override double OutgoingValue
        {
            get
            {
                lock (ctLock)
                {
                    return _outgoingValue;
                }
            }
            protected set
            {
                lock (ctLock)
                {
                    _outgoingValue = value;
                }
            }
        }
        protected override PropertyChangedEventArgs UpdateEventArgs => updateEventArgs;
        private static PropertyChangedEventArgs updateEventArgs = new PropertyChangedEventArgs("ColorTemp");

        protected override string GetWishFragment(double value)
        {
            return $"{LightProperties.ColorTemp.ToString().ToLower()}={value.ToString("F2")}";
        }

        protected override string GetWonderFragment()
        {
            return $"{LightProperties.ColorTemp.ToString().ToLower()}";
        }

        protected override LightProperties AssociatedProperty => LightProperties.ColorTemp;

    }
}
