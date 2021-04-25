using MyLights.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Bridges
{
    public interface ILightPropertiesProvider
    {
        int Index { get; }
        string Name { get; }
        IDeviceProperty<bool> PowerProperty { get; }
        IDeviceProperty<HSV> ColorProperty { get; }
        IDeviceProperty<string> ModeProperty { get; }
        IDeviceProperty<double> BrightnessProperty { get; }
        IDeviceProperty<double> ColorTempProperty { get; }
    }
}
