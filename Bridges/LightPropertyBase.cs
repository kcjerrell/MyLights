using MyLights.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Bridges
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
}
