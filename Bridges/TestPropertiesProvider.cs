using MyLights.Models;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MyLights.Bridges
{
    class TestPropertiesProvider : ILightPropertiesProvider
    {
        public TestPropertiesProvider()
        {
            nCreated += 1;
            Index = nCreated;
            Name = $"TestLight{Index}";

            if (nCreated % 2 == 0)
                ModeProperty = new TestProperty<LightMode>("Mode", LightMode.Color);
            else
                ModeProperty = new TestProperty<LightMode>("Mode", LightMode.White);
        }

        private static int nCreated = 0;

        public int Index { get; init; }

        public string Name { get; init; }

        public IDeviceProperty<bool> PowerProperty { get; } = new TestProperty<bool>("Power", true);

        public IDeviceProperty<HSV> ColorProperty { get; } = new TestProperty<HSV>("Color", new HSV(0.7, 0.7, 0.7));

        public IDeviceProperty<LightMode> ModeProperty { get; }

        public IDeviceProperty<double> BrightnessProperty { get; } = new TestProperty<double>("Brightness", 0.9);

        public IDeviceProperty<double> ColorTempProperty { get; } = new TestProperty<double>("ColorTemp", 0.9);

        class TestProperty<T> : IDeviceProperty<T>
        {
            public TestProperty(string propertyName, T initialValue)
            {
                name = propertyName;
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
