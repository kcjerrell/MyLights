using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Models
{
    //I want to remake this so it's a subclass of Light, so I can
    //just use the same LightVM class. I don't know why I did all this weird like this
    //
    public class LightGroup : ICollection<Light>
    {
        List<Light> lights = new List<Light>();

        public LightGroup(IEnumerable<Light> lights)
        {
            foreach (var light in lights)
            {
                Add(light);
            }
        }

        internal void SetColor(HSV hsv)
        {
            foreach (var light in lights)
            {
                light.SetColor(hsv);
            }
        }

        internal void SetPower(bool value)
        {
            foreach (var light in lights)
            {
                light.SetPower(value);
            }
        }

        public int Count => lights.Count;

        public bool IsReadOnly => false;

        public string Indexes
        {
            get
            {
                var indexes = from light in lights
                              select light.Index.ToString();
                return string.Join('+', indexes);
            }
        }

        internal void Ungroup()
        {
            foreach (var light in lights)
            {
                light.Ungroup();
            }

            lights.Clear();
        }

        internal void SetMode(string value)
        {
            foreach (var light in lights)
            {
                light.SetMode(value);
            }
        }

        public void Add(Light item)
        {
            item.Engroup(this, lights.Count == 0);
            lights.Add(item);
        }

        public void Clear()
        {
            lights.Clear();
        }

        public bool Contains(Light item)
        {
           return lights.Contains(item);
        }


        public void CopyTo(Light[] array, int arrayIndex)
        {
            lights.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Light> GetEnumerator()
        {
            return lights.GetEnumerator();
        }

        public bool Remove(Light item)
        {
            throw new Exception();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return lights.GetEnumerator();
        }
    }
}
