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
    internal class LightGroup // : Light, ICollection<Light>
    {
        //public LightGroup()
        //{
        //    base.color = new DpsColor();
        //    base.brightness = new DpsBrightness();
        //    base.colorTemp = new DpsColorTemp();
        //    base.mode = new DpsMode();
        //    base.power = new DpsPower();
        //}

        //List<Light> lights = new List<Light>();

        //string indexPath = "";

        //private void UpdateIndexPath()
        //{
        //    throw new NotImplementedException();

        //    //var indices = from l in lights
        //    //              select l.Index;
        //    //
        //    //indexPath = string.Join(',', indices.ToString());
        //    //
        //    //base.color.IndexPath = indexPath;
        //    //base.brightness.IndexPath = indexPath;
        //    //base.warmth.IndexPath = indexPath;
        //    //base.mode.IndexPath = indexPath;
        //    //base.power.IndexPath = indexPath;
        //}

        //public int Count => lights.Count;

        //public bool IsReadOnly => false;

        //public void Add(Light item)
        //{
        //    lights.Add(item);
        //    UpdateIndexPath();
        //}

        //public void Clear()
        //{
        //    lights.Clear();
        //    UpdateIndexPath();
        //}

        //public bool Contains(Light item)
        //{
        //    return lights.Contains(item);
        //}

        //public void CopyTo(Light[] array, int arrayIndex)
        //{
        //    lights.CopyTo(array, arrayIndex);
        //}

        //public IEnumerator<Light> GetEnumerator()
        //{
        //    return lights.GetEnumerator();
        //}

        //public bool Remove(Light item)
        //{
        //    var removed = lights.Remove(item);
        //    UpdateIndexPath();
        //    return removed;
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return lights.GetEnumerator();
        //}
    }
}
