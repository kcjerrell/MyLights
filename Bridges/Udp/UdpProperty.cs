using MyLights.Models;
using MyLights.ViewModels;
using PropertyChanged;
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
    /* The IDeviceProperties classes are going to maintain their Value as the primary, conclusive
     * value for the device. Property changes will be received and forwarded to the IDeviceProperty,
     * where instead of changing it's Value to match and notifying upward, it will schedule another 
     * request to the device, changing it to match THIS value.
    */

    public abstract class UdpProperty
    {
        private volatile bool _hasPendingChange;
        public bool HasPendingChange { get => _hasPendingChange; protected set => _hasPendingChange = value; }
        protected abstract LightProperties AssociatedProperty { get; }
        public abstract string GetOutgoingFragment(bool clearPendingStatus = false);
    }

    [DoNotNotify]
    public abstract class UdpProperty<T> : UdpProperty, IDeviceProperty<T>
    {
        public UdpProperty(Func<string,Task> immediateCallback)
        {
            this.immediateCallback = immediateCallback;
        }

        private Func<string, Task> immediateCallback;

        private T _value;

        public T Value 
        { 
            get => _value;
            protected set
            {
                _value = value;
                var handler = Updated;
                handler?.Invoke(this, UpdateEventArgs);
            }
        }

        private void UpdateDevice(T value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use this to notify the IDeviceProperty of the remote device's property's value 
        /// </summary>
        /// <param name="value">The value received from the remote device</param>
        public virtual void UpdateValue(T remoteValue, bool syncToRemote = false)
        {
            if (syncToRemote)
            {
                Value = remoteValue;
            }

            else if (!Compare(remoteValue, Value))
            {
                // request resync after period
            }
        }

        public Task Set(T newValue, bool immediate = false)
        {
            if (immediate)
            {
                Value = newValue;
                return immediateCallback(GetWishFragment(newValue));
            }

            if (ValidateValue(newValue) && !Compare(newValue, Value))
            {
                Value = newValue;

                OutgoingValue = newValue;
                HasPendingChange = true;

                OutgoingChangeRequested.Invoke(this, EventArgs.Empty);
            }

            return Task.CompletedTask;
        }

        protected virtual bool ValidateValue(T newValue)
        {
            return true;
        }

        /// <summary>
        /// This will be used to resync local values with remote values
        /// </summary>
        /// <returns></returns>
        public async Task Update()
        {

        }

        public override string GetOutgoingFragment(bool clearOutgoingStatus = false)
        {
            if (clearOutgoingStatus)
                HasPendingChange = false;

            return GetWishFragment(OutgoingValue);
        }

        public event EventHandler OutgoingChangeRequested;
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

        public UdpPower(Func<string, Task> immediateCallback) : base(immediateCallback)
        {
        }

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

        public UdpColor(Func<string, Task> immediateCallback) : base(immediateCallback)
        {
        }

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
            return $"h{color.H.ToString("F4")}s{color.S.ToString("F4")}v{color.V.ToString("F4")}";
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

    public class UdpMode : UdpProperty<LightMode>
    {
        private volatile LightMode mode;
        public override LightMode OutgoingValue
        {
            get
            {
                return mode;
            }
            protected set
            {
                mode = value;
            }
        }

        protected override string GetWishFragment(LightMode value)
        {
            return $"{LightProperties.Mode.ToString().ToLower()}={value}";
        }

        protected override string GetWonderFragment()
        {
            return $"{LightProperties.Mode.ToString().ToLower()}";
        }

        private static PropertyChangedEventArgs updateEventArgs = new PropertyChangedEventArgs("Mode");

        public UdpMode(Func<string, Task> immediateCallback) : base(immediateCallback)
        {
        }

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

        protected override bool ValidateValue(double newValue)
        {
            return newValue >= 10.0 && newValue <= 1000.0;
        }

        protected override PropertyChangedEventArgs UpdateEventArgs => updateEventArgs;
        private static PropertyChangedEventArgs updateEventArgs = new PropertyChangedEventArgs("Brightness");

        public UdpBrightness(Func<string, Task> immediateCallback) : base(immediateCallback)
        {
        }

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

        protected override bool ValidateValue(double newValue)
        {
            return newValue >= 0 && newValue <= 1000;
        }

        protected override PropertyChangedEventArgs UpdateEventArgs => updateEventArgs;
        private static PropertyChangedEventArgs updateEventArgs = new PropertyChangedEventArgs("ColorTemp");

        public UdpColorTemp(Func<string, Task> immediateCallback) : base(immediateCallback)
        {
        }

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
