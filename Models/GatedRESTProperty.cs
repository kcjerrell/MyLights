using Flurl;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyLights.Models
{
    internal class GatedRESTProperty<T> where T : IComparable<T>
    {
        public GatedRESTProperty(BuildURI<T> uriBuilderFunction, T initialValue, UpdateCallback<T> updateCallback)
        {
            BuildURI = uriBuilderFunction;
            changeTo = initialValue;
            Value = initialValue;
            UpdateCallback = updateCallback;
        }

        public string PropURL { get; }
        public BuildURI<T> BuildURI { get; }

        public UpdateCallback<T> UpdateCallback { get; }

        public T Value { get; }

        private T changeTo
        {
            get
            {
                lock (lockObject)
                    return changeTo;
            }

            set
            {
                lock (lockObject)
                    changeTo = value;
            }
        }

        private object lockObject = new object();
        private bool updating;

        public async Task Set(T value)
        {
            if (updating)
            {
                changeTo = value;
            }
            else
            {
                updating = true;
                Update(value);
            }
        }

        private async void Update(T newValue)
        {
            if (updating)
                return;

            T to = changeTo;


        }

        public delegate string BuildURI<in T>(T value);
        public delegate void UpdateCallback<T>(T value);
    }
