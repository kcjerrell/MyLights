using MyLights.Models;
using MyLights.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Bridges.Udp2
{
    public abstract class LightProperty
    {
        private volatile bool _hasPendingChange;
        public bool HasPendingChange { get => _hasPendingChange; protected set => _hasPendingChange = value; }
        protected abstract LightProperties AssociatedProperty { get; }
        public abstract string GetProperty(bool clearPendingStatus = false);
    }

    public abstract class LightProperty<T> : LightProperty, IDeviceProperty<T>
    {
        private T _value;

        public T Value
        {
            get => _value;
            protected set
            {
                _value = value;
                RaiseUpdated();
            }
        }

        public virtual Task Set(T value, bool immediate = false)
        {
            Value = value;
            HasPendingChange = true;
            ChangeRequested.Invoke(this, false);

            return Task.CompletedTask;
        }

        public Task Update()
        {
            throw new NotImplementedException();
        }

        public virtual void UpdateValue(T value)
        {
            Value = value;
        }

        protected void RaiseUpdated()
        {
            var handler = Updated;
            handler?.Invoke(this, UpdateEventArgs);
        }

        protected abstract PropertyChangedEventArgs UpdateEventArgs { get; }

        public event PropertyChangeRequestedEventHandler<T> ChangeRequested;
        public event PropertyChangedEventHandler Updated;
    }

    public delegate void PropertyChangeRequestedEventHandler<T>(LightProperty<T> sender, bool immediate);

    public class Udp2LightProperties
    {
        public class Power : LightProperty<bool>
        {
            static PropertyChangedEventArgs updateEventArgs = new("Power");
            protected override PropertyChangedEventArgs UpdateEventArgs => updateEventArgs;

            protected override LightProperties AssociatedProperty => LightProperties.Power;

            public override string GetProperty(bool clearPendingStatus = false)
            {
                if (clearPendingStatus)
                    HasPendingChange = false;

                return $"power/{Value}";
            }
        }

        public class Mode : LightProperty<LightMode>
        {
            public void UpdateValue(string mode)
            {
                base.UpdateValue(Helpers.StringToMode(mode));
            }

            static PropertyChangedEventArgs updateEventArgs = new("Mode");
            protected override PropertyChangedEventArgs UpdateEventArgs => updateEventArgs;

            protected override LightProperties AssociatedProperty => LightProperties.Mode;

            public override string GetProperty(bool clearPendingStatus = false)
            {
                if (clearPendingStatus)
                    HasPendingChange = false;

                return $"mode/{Helpers.ModeToString(Value)}";
            }
        }

        public class Color : LightProperty<HSV>
        {
            private int h;
            private int s;
            private int v;

            public override Task Set(HSV value, bool immediate = false)
            {
                h = (int)(value.H * 360);
                s = (int)(value.S * 1000);
                v = (int)(value.V * 1000);

                return base.Set(value, immediate);
            }

            public void UpdateValue(string color)
            {
                var split = color.Split('-');

                h = int.Parse(split[0]);
                s = int.Parse(split[1]);
                v = int.Parse(split[2]);

                UpdateValue(ToHSV(h, s, v));
            }

            static HSV ToHSV(int hue, int sat, int val)
            {
                double h = hue / 360.0;
                double s = sat / 1000.0;
                double v = val / 1000.0;

                return new HSV(h, s, v);
            }

            static PropertyChangedEventArgs updateEventArgs = new("Color");
            protected override PropertyChangedEventArgs UpdateEventArgs => updateEventArgs;
            protected override LightProperties AssociatedProperty => LightProperties.Color;

            public override string GetProperty(bool clearPendingStatus = false)
            {
                if (clearPendingStatus)
                    HasPendingChange = false;

                int h = (int)(Value.H * 360);
                int s = (int)(Value.S * 1000);
                int v = (int)(Value.V * 1000);

                return $"color/{h}-{s}-{v}";
            }
        }

        public class Brightness : LightProperty<double>
        {
            static PropertyChangedEventArgs updateEventArgs = new("Brightness");
            protected override PropertyChangedEventArgs UpdateEventArgs => updateEventArgs;
            protected override LightProperties AssociatedProperty => LightProperties.Brightness;

            public override string GetProperty(bool clearPendingStatus = false)
            {
                if (clearPendingStatus)
                    HasPendingChange = false;

                return $"brightness/{Value.ToString("F2")}";
            }
        }

        public class ColorTemp : LightProperty<double>
        {
            static PropertyChangedEventArgs updateEventArgs = new("ColorTemp");
            protected override PropertyChangedEventArgs UpdateEventArgs => updateEventArgs;
            protected override LightProperties AssociatedProperty => LightProperties.ColorTemp;
            public override string GetProperty(bool clearPendingStatus = false)
            {
                if (clearPendingStatus)
                    HasPendingChange = false;

                return $"colortemp/{Value.ToString("F2")}";
            }
        }

        public class Scene : LightProperty<string>
        {     
            static PropertyChangedEventArgs updateEventArgs = new("ColorTemp");

            protected override PropertyChangedEventArgs UpdateEventArgs => updateEventArgs;
            protected override LightProperties AssociatedProperty => LightProperties.Scene;

            public override string GetProperty(bool clearPendingStatus = false)
            {
                if (clearPendingStatus)
                    HasPendingChange = false;

                return Value;
            }
        }
    }
}