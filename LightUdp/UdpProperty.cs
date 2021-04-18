using MyLights.Models;
using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyLights.LightUdp
{
    public abstract class UdpProperty<T> : IDeviceProperty<T> where T : IEquatable<T>
    {
        public UdpProperty(Client client, string resId)
        {
            this.client = client;
            this.ResourceId = resId;

            Update();
        }

        private const int throttleDelay = 50;

        private Client client;
        private T nextValue;
        private bool isThrottled;
        protected string ResourceId { get; }

        private T _value;
        public T Value
        {
            get => _value;
            protected set
            {
                _value = value;
                OnUpdated();
            }
        }

        protected abstract PropertyChangedEventArgs UpdateEventArgs { get; }

        private void OnUpdated()
        {
            var handler = Updated;
            handler?.Invoke(this, UpdateEventArgs);
        }

        public void UpdateValue(T value)
        {
            Value = value;
        }

        public event PropertyChangedEventHandler Updated;

        public async Task Set(T newValue)
        {
            nextValue = newValue;

            if (!isThrottled)
            {
                isThrottled = true;
                await client.SendMessage(GetWishMessage(newValue));

                await Task.Delay(throttleDelay);

                isThrottled = false;

                if (Compare(nextValue, newValue))
                    Set(nextValue);
            }
        }

        public async Task Update()
        {
            await client.SendMessage(GetWonderMessage());
        }

        protected virtual bool Compare(T first, T second)
        {
            if (first == null)
                return false;
            return first.Equals(second);
        }

        protected abstract LightDgram GetWishMessage(T value);
        protected abstract LightDgram GetWonderMessage();

    }

    public class UdpPower : UdpProperty<bool>
    {
        public UdpPower(Client client, string resId)
            : base(client, resId)
        {
        }

        private static PropertyChangedEventArgs updateEventArgs = new PropertyChangedEventArgs("Power");

        protected override PropertyChangedEventArgs UpdateEventArgs => updateEventArgs;

        protected override LightDgram GetWishMessage(bool value)
        {
            return new LightDgram(DgramVerbs.Wish, ResourceId, LightProperties.Power, value.ToString());
        }

        protected override LightDgram GetWonderMessage()
        {
            return new LightDgram(DgramVerbs.Wonder, ResourceId, LightProperties.Power);
        }
    }

    public class UdpColor : UdpProperty<HSV>
    {
        public UdpColor(Client client, string resId) : base(client, resId)
        {
        }
        private static Regex pattern = new Regex("h([0-9.]+)s([0-9.]+)v([0-9.]+)");

        private static PropertyChangedEventArgs updateEventArgs = new("Color");
        protected override PropertyChangedEventArgs UpdateEventArgs => throw new NotImplementedException();

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

        protected override LightDgram GetWishMessage(HSV value)
        {
            return new LightDgram(DgramVerbs.Wish, ResourceId, LightProperties.Color, EncodeColor(value));
        }

        protected override LightDgram GetWonderMessage()
        {
            return new LightDgram(DgramVerbs.Wonder, ResourceId, LightProperties.Color);
        }
    }

    public class UdpMode : UdpProperty<string>
    {
        public UdpMode(Client client, string resId) : base(client, resId)
        {
        }
        private static PropertyChangedEventArgs updateEventArgs = new PropertyChangedEventArgs("Mode");

        protected override PropertyChangedEventArgs UpdateEventArgs => updateEventArgs;

        protected override LightDgram GetWishMessage(string value)
        {
            return new LightDgram(DgramVerbs.Wish, ResourceId, LightProperties.Mode, value);
        }

        protected override LightDgram GetWonderMessage()
        {
            return new LightDgram(DgramVerbs.Wonder, ResourceId, LightProperties.Mode);
        }
    }

    public class UdpBrightness : UdpProperty<double>
    {
        public UdpBrightness(Client client, string resId) : base(client, resId)
        {
        }

        private static PropertyChangedEventArgs updateEventArgs = new PropertyChangedEventArgs("Brightness");

        protected override PropertyChangedEventArgs UpdateEventArgs => updateEventArgs;

        protected override LightDgram GetWishMessage(double value)
        {
            return new LightDgram(DgramVerbs.Wish, ResourceId, LightProperties.Brightness, value.ToString());
        }

        protected override LightDgram GetWonderMessage()
        {
            return new LightDgram(DgramVerbs.Wonder, ResourceId, LightProperties.Brightness);
        }
    }

    public class UdpColorTemp : UdpProperty<double>
    {
        public UdpColorTemp(Client client, string resId) : base(client, resId)
        {
        }

        private static PropertyChangedEventArgs updateEventArgs = new PropertyChangedEventArgs("ColorTemp");

        protected override PropertyChangedEventArgs UpdateEventArgs => updateEventArgs;

        protected override LightDgram GetWishMessage(double value)
        {
            return new LightDgram(DgramVerbs.Wish, ResourceId, LightProperties.ColorTemp, value.ToString());
        }

        protected override LightDgram GetWonderMessage()
        {
            return new LightDgram(DgramVerbs.Wonder, ResourceId, LightProperties.ColorTemp);
        }
    }
}
