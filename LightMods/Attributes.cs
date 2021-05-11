using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.LightMods
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public abstract class LightEffectAttribute : Attribute
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
    sealed class MultiLightEffectAttribute : LightEffectAttribute
    {
        public MultiLightEffectAttribute(string name, string iconPath) : base(name, iconPath)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class SingleLightEffectAttribute : LightEffectAttribute
    {
        public SingleLightEffectAttribute(string name, string iconPath) : base(name, iconPath)
        {
        }
    }
}
