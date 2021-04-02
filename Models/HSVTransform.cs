using MyLights.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Models
{
    public abstract class HSVTransform
    {

        public HSV Transform(HSV hsv)
        {
            return TransformOverride(hsv);
        }

        protected abstract HSV TransformOverride(HSV hsv);

    }

    public class HSVTranslate : HSVTransform, INotifyPropertyChanged
    {
        public HSVTranslate()
        {

        }

        public HSVTranslate(double hOffset, double sOffset, double vOffset, bool isHueClamped = false)
        {
            HueOffset = hOffset;
            SaturationOffset = sOffset;
            ValueOffset = vOffset;

            IsHueClamped = isHueClamped;
        }

        public double HueOffset { get; set; }
        public double SaturationOffset { get; set; }
        public double ValueOffset { get; set; }
        public bool IsHueClamped { get; set; }
        public double HueClamp { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected override HSV TransformOverride(HSV hsv)
        {
            double h;
            if (IsHueClamped)
            {
                //not sure how to do clamping on different colors yet
                h = hsv.H + HueOffset;
                h = h.Clamp(0, 1);
            }
            else
            {
                h = hsv.H + HueOffset;
                h = h % 1.0; // I love modulo <3%
            }

            double s = hsv.S + SaturationOffset;
            s = s.Clamp(0, 1);

            double v = hsv.V + ValueOffset;
            v = v.Clamp(0, 1);

            return new HSV(h, s, v);
        }
    }
}
