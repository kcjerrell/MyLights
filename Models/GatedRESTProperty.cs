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
    internal class GatedRESTProperty<T> where T : IEquatable<T>
    {
        public GatedRESTProperty(string url, QueryBuilder<T> queryBuilder, ResponseSelector<T> responseSelector)
        {
            PropURL = url;
            QueryBuilder = queryBuilder;
            ResponseSelector = responseSelector;
        }

        public string PropURL { get; }
        public QueryBuilder<T> QueryBuilder { get; }

        public ResponseSelector<T> ResponseSelector { get; }

        public T Value { get; private set; }
        private T changeTo;

        private bool updating;

        public void Set(T value)
        {
            if (updating)
            {
                changeTo = value;
            }
            else
            {
                Update();
            }
        }

        public async Task<T> GetValue()
        {           
            var res = await PropURL.GetJsonAsync<JsonDpsRoot>();
            Value = ResponseSelector(res);
            return Value;
        }

        private async void Update()
        {
            updating = true;
            T to = changeTo;

            var query = QueryBuilder(to);
            var res = await string.Concat(PropURL, query).GetJsonAsync<JsonDpsRoot>();
            Value = ResponseSelector(res);

           // if (Value == changeTo)
                Update();
        }
    }
    public delegate string QueryBuilder<in T>(T value);
    public delegate T ResponseSelector<out T>(JsonDpsRoot response);
}
