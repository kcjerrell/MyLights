using MyLights.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Bridges
{
    public interface ILightPropertiesProvider : INotifyPropertyChanged
    {
        int Index { get; }
        string Id { get; }
        IDeviceProperty<bool> PowerProperty { get; }
        IDeviceProperty<HSV> ColorProperty { get; }
        IDeviceProperty<LightMode> ModeProperty { get; }
        IDeviceProperty<double> BrightnessProperty { get; }
        IDeviceProperty<double> ColorTempProperty { get; }

        IDeviceProperty<string> SceneProperty { get; }
    }
}
