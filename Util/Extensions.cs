using MyLights.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyLights.Util
{
    public static class Extensions
    {

        public static Color ToColor(this JsonColor jsonColor)
        {
            return Color.FromRgb((byte)jsonColor.r, (byte)jsonColor.g, (byte)jsonColor.b);
        }

    }
}
