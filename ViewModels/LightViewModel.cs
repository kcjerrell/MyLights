using MyLights.Models;
using MyLights.Util;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyLights.ViewModels
{
    /*
     * Trying to decide how to keep sliders smooth and still syncronize
     * Since I seem to be going the way of using async/await and delays/gates
     * around syncing with remote resources, I think I'll switch the VM to use
     * a cached value, and - ...
     * well there's couple ways to go, and they depend on which value is or should 
     * be the ultimate/target value for a property
     * using brightness as the example here...
     * 
     * vm._brightness
     *      - with this as the definitive value, I would probably start a timer
     *        (aka an async task with a Task.Delay()) to follow up on changes
     *        sent to the the model. maybe.
     *      - the other issue is getting the initial value... with the udpprovider,
     *        I don't think it will be an issue because the Light isn't constructed 
     *        until all the current values have been gathered. I'm not sure what would
     *        happen with the REST provider, but then again, I might never use that again
     * 
     * light.Brightness
     *      - this is how it works now, and that's why the sliders are unresponsive
     *      - I'm no sure how that issue can be dealt with, since the slider value
     *        will have to keep reverting back to this value. It probably won't even 
     *        matter though. I'll just go with the first option above
     * 
     * bulb-01.brightness
     */

    public class LightViewModel : INotifyPropertyChanged
    {
        public LightViewModel(Light light)
        {
            this.Light = light;
            light.PropertyChanged += Light_PropertyChanged; 

            Name = light.Name;
        }

        public Light Light { get; init; }
        public string Name { get; init; }
        public LightMode Mode
        {
            get => Light.Mode;
            set
            {
                Light.SetMode(value);
            }
        }
        public bool Power
        {
            get => Light.Power;
            set
            {
                Light.SetPower(value);
            }
        }
        public double Brightness
        {
            get => Light.Brightness;
            set
            {
                Light.SetBrightness(value);
            }
        }
        public double ColorTemp
        {
            get => Light.ColorTemp;
            set
            {
                Light.SetColorTemp(value);
            }
        }
        public HSV Color
        {
            get => Light.Color;
            set
            {
                UpdateColor(value.H, value.S, value.V);
            }
        }
        [DependsOn("Color")]
        public double H
        {
            get => Color.H; // this.hsv.H;
            set
            {
                UpdateColor(h: value);
            }
        }
        [DependsOn("Color")]
        public double S
        {
            get => Color.S; // this.hsv.S;
            set
            {
                UpdateColor(s: value);
            }
        }
        [DependsOn("Color")]
        public double V
        {
            get => Color.V; // this.hsv.V;
            set
            {
                UpdateColor(v: value);
            }
        }

        public bool IsSelected { get; set; }

        private void Light_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, e);
            
            if (e.PropertyName == "Color")
            {
                handler?.Invoke(this, new PropertyChangedEventArgs("H"));
                handler?.Invoke(this, new PropertyChangedEventArgs("S"));
                handler?.Invoke(this, new PropertyChangedEventArgs("V"));
            }
        }

        private void UpdateColor(double h = -1.0, double s = -1.0, double v = -1.0)
        {
            var hsv = Light.Color;

            if (h == -1)
                h = hsv.H;
            if (s == -1)
                s = hsv.S;
            if (v == -1)
                v = hsv.V;

            Light.SetColor(new HSV(h, s, v));
        }

        public override string ToString()
        {
            return $"(LVM: {Name}-{(Power ? "on" : "off")}-{Mode}-{Color}";
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ModeOptions
    {
        public static List<string> ListOptions()
        {
            var options = new List<string>();
            options.Add("color");
            options.Add("white");
            return options;
        }
    }
}