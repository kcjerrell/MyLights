using MyLights.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MyLights.Models
{
    public class Scene
    {
        public Scene()
        {
            Stops.CollectionChanged += Stops_CollectionChanged;
        }

        public Scene(string encoded)
            : this()
        {
            Decode(encoded);
        }

        bool supressEvent = false;

        public string Encoded { get; private set; }

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

            if (!supressEvent)
                RaiseSceneChanged();
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!supressEvent)
                RaiseSceneChanged();
        }

        public ObservableCollection<SceneStop> Stops { get; } = new();

        private string Encode()
        {
            var stops = from stop in Stops
                        select stop.Encode();
            return $"07{string.Join("", stops)}";
        }

        public void Decode(string encoded)
        {
            if (encoded == Encoded)
                return;

            supressEvent = true;
            Stops.Clear();

            if (encoded != null)
            {
                for (int i = 2; i < encoded.Length; i += 26)
                {
                    string sub = encoded.Substring(i, 26);
                    Stops.Add(SceneStop.Decode(sub));
                }
            }

            supressEvent = false;
            RaiseSceneChanged();
        }

        public event EventHandler SceneChanged;
        private void RaiseSceneChanged()
        {
            Encoded = Encode();

            var handler = SceneChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }


    public class SceneStop : INotifyPropertyChanged
    {
        public SceneStop()
        {
            this.Color = new HSV(1, 1, 1);
            this.Speed = 40;
            this.Transition = SceneTransition.Breath;
        }

        public SceneStop(HSV color, int speed, SceneTransition transition)
        {
            this.Color = color;
            this.Speed = speed.Clamp(40, 100);
            this.Transition = transition;
        }

        public HSV Color { get; set; }
        public double Speed { get; set; }
        public SceneTransition Transition { get; set; }

        public string Encode()
        {
            string speed = ((int)Speed).ToString("X2");
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
