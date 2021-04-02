using MyLights.Models;
using MyLights.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyLights.ViewModels
{
    public class TransformCascadeViewModel : INotifyPropertyChanged
    {
        public TransformCascadeViewModel()
        {
            var lb = new LightBridge();
            lvms = lb.LightVMs;
            lvms.CollectionChanged += (s, e) => CreateTransforms();
        }

        ObservableCollection<LightViewModel> lvms;

        Dictionary<LightViewModel, HSVTranslate> lights;
        public List<HSVTranslate> Transforms { get; set; }

        Color _color;

        private void CreateTransforms()
        {          
            var lt = (from lvm in lvms
                      select KeyValuePair.Create(lvm, new HSVTranslate()));
            lights = new Dictionary<LightViewModel, HSVTranslate>(lt);

            Transforms = lights.Values.ToList();

            foreach (var t in Transforms)
            {
                t.PropertyChanged += (s, e) => UpdateLights();
            }
        }

        public Color Color {
            get => _color;
            set
            {
                _color = value;
                UpdateLights();
            }
        }

        private void UpdateLights()
        {
            HSV hsv = this.Color.ToHSV();

            foreach (var (lvm, trans) in lights)
            {
                lvm.Color = trans.Transform(hsv);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged; // <3 fody
    }
}
