using Flurl;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Models
{
    // I would need DpsProperty<string>, DpsProperty<bool>, and DpsProperty<HSV>
    // I guess they can just be subtypes
    internal abstract class DpsProperty<T> where T : IEquatable<T>
    {
        // #review
        // this won't work right, maybe. Well, I guess it could.
        // I was thinking about making the DpsProperties static
        //either
        // A:
        //   - Have DpsProperty instances on each Light instance
        //   - Generate new instances for any LightGroups that are created
        // B:
        //   - Use a static instance for each dpsproperty
        //   - pass in the index string to make the calls
        // B has syncronization issues, possibly, A just makes a lot of objects. Depending on
        // what might come later, that could be an issue. Maybe. Maybe not. let's go A for now.

        // maybe having static/const properties on the derived types might work.

        internal DpsProperty(string propertyPath, string indexPath, T initialValue = default)
        {
            this.propertyPath = propertyPath;
            this.indexPath = indexPath;
            this.Value = initialValue;
        }

        const string urlBase = @"http://localhost:1337/bulbs";

        private string indexPath;
        private string propertyPath;
        private bool requestInProgress = false;
        private T nextValue;


        /// <summary>
        /// Gets the last value received from server
        /// Use Update() to recheck
        /// </summary>
        public T Value { get; protected set; }

        internal virtual async void Set(T newValue)
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

            if (!requestInProgress && !Compare(newValue, Value))
            {
                requestInProgress = true;

                string url = urlBase
                            .AppendPathSegment(indexPath)
                            .AppendPathSegment(propertyPath)
                            .SetQueryParams(GetQuery(newValue));

                var res = await url.GetJsonAsync<JsonDpsRoot>();

                var dps = res.Data[0].Dps;
                this.Value = GetValue(dps);

                requestInProgress = false;
                Set(nextValue);
            }
        }



        internal virtual async Task<T> Update()
        {
            // #fix 
            // this won't work with updating more than one index
            //      - consider changing return type to T[] ?
            // this should block if a Set is already executing
            //      - (but it doesn't [yet])

            string url = urlBase
                         .AppendPathSegment(indexPath)
                         .AppendPathSegment(propertyPath);

            var res = await url.GetJsonAsync<JsonDpsRoot>();

            var dps = res.Data[0].Dps;
            this.Value = GetValue(dps);

            return this.Value;
        }

        internal virtual bool Compare(T first, T second)
        {
            return first.Equals(second);
        }

        protected abstract T GetValue(JsonDps dps);
        protected abstract object GetQuery(T value);
    }

    internal class DpsMode : DpsProperty<string>
    {
        public DpsMode(string indexPath, string initialValue = default) : base("mode", indexPath, initialValue)
        {
        }

        protected override object GetQuery(string value)
        {
            string mode;

            if (value == "colour")
                mode = "color";
            else
                mode = value;

            return new { v = mode };
        }

        protected override string GetValue(JsonDps dps)
        {
            return dps.Mode;
        }
    }

    internal class DpsPower : DpsProperty<bool>
    {
        public DpsPower(string indexPath, bool initialValue = default) : base("power", indexPath, initialValue)
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

    internal class DpsColor : DpsProperty<HSV>
    {
        public DpsColor(string indexPath, HSV initialValue = default) : base("color", indexPath, initialValue)
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

        internal override bool Compare(HSV a, HSV b)
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

    internal class DpsBrightness : DpsProperty<double>
    {
        public DpsBrightness(string indexPath, double initialValue = 0) : base("brightness", indexPath, initialValue)
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

        internal override void Set(double newValue)
        {
            Value = newValue;
        }

        internal override Task<double> Update()
        {
            return Task.FromResult<double>(Value);
        }
    }

    internal class DpsColorTemp : DpsProperty<double>
    {
        public DpsColorTemp(string indexPath, double initialValue = 0) : base("colorTemp", indexPath, initialValue)
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

        internal override void Set(double newValue)
        {
            Value = newValue;
        }

        internal override Task<double> Update()
        {
            return Task.FromResult<double>(Value);
        }
    }
}
