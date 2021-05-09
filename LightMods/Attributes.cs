using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.LightMods
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class LightEffectAttribute : Attribute
    {
        public LightEffectAttribute(string name, string iconPath)
        {
            this.Name = name;
            this.IconPath = iconPath;
        }

        public string Name { get; init; }
        public string IconPath { get; init; }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class MultiLightEffectAttribute : Attribute
    {
        public MultiLightEffectAttribute(string name, string iconPath)
        {
            this.Name = name;
            this.IconPath = iconPath;
        }

        public string Name { get; init; }
        public string IconPath { get; init; }
    }
}
