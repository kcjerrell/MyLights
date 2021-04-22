using MyLights.Models;
using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Util
{
    class TestLightBridge : ILightBridge
    {
        public ObservableCollection<Light> Lights => lights;

        public ObservableCollection<LightViewModel> LightVMs => lightVMs;

        public bool TryFindBulb(BulbRef key, out Light light)
        {
            light = null;
            return false;
        }

        static TestLightBridge()
        {
            for (int i = 0; i < 3; i++)
            {
                var props = new TestPropertiesProvider();
                var light = new Light(props);

                lights.Add(light);

                lightVMs.Add(new LightViewModel(light));
            }
        }

        static ObservableCollection<Light> lights = new();

        static ObservableCollection<LightViewModel> lightVMs = new();
    }

    class TestPropertiesProvider : ILightPropertiesProvider
    {
        public int Index => 1;

        public string Name => "TestLight";

        public IDeviceProperty<bool> PowerProperty { get; } = new TestProperty<bool>("Power", true);

        public IDeviceProperty<HSV> ColorProperty { get; } = new TestProperty<HSV>("Color", new HSV(0.7, 0.7, 0.7));

        public IDeviceProperty<string> ModeProperty { get; } = new TestProperty<string>("Mode", "color");

        public IDeviceProperty<double> BrightnessProperty { get; } = new TestProperty<double>("Brightness", 0.9);

        public IDeviceProperty<double> ColorTempProperty { get; } = new TestProperty<double>("ColorTemp", 0.9);

        class TestProperty<T> : IDeviceProperty<T> where T : IEquatable<T>
        {
            public TestProperty(string propertyName, T initialValue)
            {
                this.name = propertyName;
                updatedEventArgs = new PropertyChangedEventArgs(name);
                Value = initialValue;
            }

            private T _value;
            private PropertyChangedEventArgs updatedEventArgs;
            private string name;

            public T Value
            {
                get => _value;
                set
                {
                    _value = value;
                    OnUpdated();
                }
            }

            private void OnUpdated()
            {
                var handler = Updated;
                handler?.Invoke(this, updatedEventArgs);
            }

            public event PropertyChangedEventHandler Updated;

            public async Task Set(T value)
            {
                Value = value;
            }

            public async Task Update()
            {

            }
        }
    }
}
