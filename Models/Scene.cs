using MyLights.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Models
{
    public class Scene
    {
        public Scene()
        {
            Stops.CollectionChanged += Stops_CollectionChanged;
        }

        string lastDecode = "";
        bool supressChangedEvent = false;

        private void Stops_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SceneStop item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (SceneStop item in e.OldItems)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }

            if (!supressChangedEvent)
            {
                var handler = SceneChanged;
                handler?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var handler = SceneChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }

        public Scene(IEnumerable<SceneStop> sceneStops)
        {
            foreach (var ss in sceneStops)
            {
                Stops.Add(ss);
            }
        }

        public ObservableCollection<SceneStop> Stops { get; } = new();

        public string Encode()
        {
            var stops = from stop in Stops
                        select stop.Encode();
            return $"07{string.Join("", stops)}";
        }

        public void Decode(string encoded)
        {
            if (lastDecode != encoded)
            {
                supressChangedEvent = true;
                Stops.Clear();

                for (int i = 2; i < encoded.Length; i += 26)
                {
                    string sub = encoded.Substring(i, 26);
                    Stops.Add(SceneStop.Decode(sub));
                }

                supressChangedEvent = false;

                var handler = SceneChanged;
                handler?.Invoke(this, EventArgs.Empty);
            }

            lastDecode = encoded;
        }

        public event EventHandler SceneChanged;
    }


    public class SceneStop : INotifyPropertyChanged
    {
        public SceneStop(HSV color, int speed, SceneTransition transition)
        {
            this.Color = color;
            this.Speed = speed.Clamp(40, 100);
            this.Transition = transition;
        }

        public HSV Color { get; set; }
        public int Speed { get; set; }
        public SceneTransition Transition { get; set; }

        public string Encode()
        {
            string speed = Speed.ToString("X2");
            string transition = ((int)(Transition)).ToString();
            string color = Color.ToTuya();

            return $"{speed}{speed}0{transition}{color}00000000";
        }

        public static SceneStop Decode(string sceneStop)
        {
            int speed = int.Parse(sceneStop.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            SceneTransition transition = (SceneTransition)int.Parse(sceneStop.Substring(5, 1));
            HSV color = HSV.FromTuya(sceneStop.Substring(6, 12));

            return new SceneStop(color, speed, transition);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return $"{Transition} - Speed: {Speed} - Color: {Color}";
        }
    }

    public enum SceneTransition
    {
        None = 0,
        Flash = 1,
        Breath = 2,
        Static = 3,
    }
}
