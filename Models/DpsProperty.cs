﻿using Flurl;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Models
{
    // I would need DpsProperty<string>, DpsProperty<bool>, and DpsProperty<HSV>
    // I guess they can just be subtypes
    public abstract class DpsProperty<T> : IDeviceProperty<T>
    {   
        public DpsProperty(string propertyPath, string indexPath = "", T initialValue = default)
        {
            this.propertyPath = propertyPath;
            this.IndexPath = indexPath;
            this.Value = initialValue;

            this.eventArgs = new PropertyChangedEventArgs(propertyPath);
        }

        const string urlBase = @"http://localhost:1337/bulbs";

        private string propertyPath;
        private bool requestInProgress = false;
        private T nextValue;

        private PropertyChangedEventArgs eventArgs;
        private T _value;

        public string IndexPath { get; set; }
        /// <summary>
        /// Gets the last value received from server
        /// Use Update() to recheck
        /// </summary>
        public T Value
        {
            get => _value;
            protected set
            {
                _value = value;
                OnUpdated();
            }
        }

        public event PropertyChangedEventHandler Updated;

        private void OnUpdated()
        {
            var handler = Updated;
            handler?.Invoke(this, eventArgs);
        }

        public virtual async Task Set(T newValue, bool immediate = false)
        {
            // #review 
            // should this be a lock or other sync primitive? I don't know if it matters for async.
            // oh well, will worry later
            // actually, maybe some kind of job queue
            // I feel like this could be bad but I don't think it will matter right now                                                

            //please leave a message and I'll call you back

            //The issue here is that the slider in wpf generates thousands of updates a second
            //potentially opening up impossible amounts of get requests. Which is bad.

            // Slider calls Set()
            // If no request is in progress, start one
            // if one is, leave a message (only save one message at a time, the latest one)
            // when a request finishes, see if the last message is different than the current value
            // start a new request

            //maybe I'm thinking too hard

            nextValue = newValue;

            if (!requestInProgress && !Compare(newValue, Value) && !string.IsNullOrEmpty(IndexPath))
            {
                requestInProgress = true;

                string url = urlBase
                            .AppendPathSegment(IndexPath)
                            .AppendPathSegment(propertyPath)
                            .SetQueryParams(GetQuery(newValue));

                var res = await url.GetJsonAsync<JsonDpsRoot>();

                var dps = res.Data[0].Dps;
                if (dps != null)
                    this.Value = GetValue(dps);

                requestInProgress = false;
                Set(nextValue, immediate: false);
            }
        }

        public virtual async Task Update()
        {
            // #fix 
            // this won't work with updating more than one index
            //      - consider changing return type to T[] ?
            // this should block if a Set is already executing
            //      - (but it doesn't [yet])

            if (string.IsNullOrEmpty(IndexPath))
                return;



            string url = urlBase
                         .AppendPathSegment(IndexPath)
                         .AppendPathSegment(propertyPath);

            var res = await url.GetJsonAsync<JsonDpsRoot>();

            var dps = res.Data[0].Dps;
            this.Value = GetValue(dps);

            return;
        }

        public virtual bool Compare(T first, T second)
        {
            if (first == null)
                return false;
            return first.Equals(second);
        }

        protected abstract T GetValue(JsonDps dps);
        protected abstract object GetQuery(T value);
    }

    public class DpsMode : DpsProperty<LightMode>
    {
        public DpsMode(string indexPath = "", LightMode initialValue = default) : base("mode", indexPath, initialValue)
        {
        }

        protected override object GetQuery(LightMode value)
        {
            return new { v = value };
        }

        protected override LightMode GetValue(JsonDps dps)
        {
                return dps.Mode;
        }
    }

    public class DpsPower : DpsProperty<bool>
    {
        public DpsPower(string indexPath = "", bool initialValue = default) : base("power", indexPath, initialValue)
        {
        }

        protected override object GetQuery(bool value)
        {
            return new { v = value };
        }

        protected override bool GetValue(JsonDps dps)
        {
            return dps.Power;
        }
    }

    public class DpsColor : DpsProperty<HSV>
    {
        public DpsColor(string indexPath = "", HSV initialValue = default) : base("color", indexPath, initialValue)
        {
        }

        protected override object GetQuery(HSV value)
        {
            //var query = new Dictionary<string, string>(3);
            //query.Add("h", value.H.ToString());
            //query.Add("s", value.S.ToString());
            //query.Add("v", value.V.ToString());
            //
            //return query;

            return new { h = value.H, s = value.S, v = value.V };
        }

        protected override HSV GetValue(JsonDps dps)
        {
            return dps.Color;
        }

        public override bool Compare(HSV a, HSV b)
        {
            if (Math.Abs(a.H - b.H) > 0.01)
                return false;
            if (Math.Abs(a.S - b.S) > 0.01)
                return false;
            if (Math.Abs(a.V - b.V) > 0.01)
                return false;

            return true;
        }
    }

    public class DpsBrightness : DpsProperty<double>
    {
        public DpsBrightness(string indexPath = "", double initialValue = 0) : base("brightness", indexPath, initialValue)
        {
        }

        protected override object GetQuery(double value)
        {
            throw new NotImplementedException();
        }

        protected override double GetValue(JsonDps dps)
        {
            throw new NotImplementedException();
        }

        public async override Task Set(double newValue, bool immediate = false)
        {
            Value = newValue;
        }

        public async override Task Update()
        {
            return;// Task.FromResult<double>(Value);
        }
    }

    public class DpsColorTemp : DpsProperty<double>
    {
        public DpsColorTemp(string indexPath = "", double initialValue = 0) : base("warmth", indexPath, initialValue)
        {
        }

        protected override object GetQuery(double value)
        {
            throw new NotImplementedException();
        }

        protected override double GetValue(JsonDps dps)
        {
            throw new NotImplementedException();
        }

        public async override Task Set(double newValue, bool immediate = false)
        {
            Value = newValue;
        }

        public async override Task Update()
        {
            return;// Task.FromResult<double>(Value);
        }
    }
}
