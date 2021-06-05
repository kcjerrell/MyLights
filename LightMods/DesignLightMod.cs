using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.LightMods
{
    [SingleLightEffect("Design Light Effect", "")]
    [MultiLightEffect("Design Light Effect", "")]
    public class DesignLightMod : ILightEffect
    {
        public DesignLightMod()
        {
            settings.Add(numberSettingA);
            settings.Add(numberSettingB);
        }

        private List<PluginSetting> settings = new();
        private NumericPluginSetting numberSettingA = new(5, 0, 10, "Designable Setting A");
        private NumericPluginSetting numberSettingB = new(80, 0, 100, "Designable Setting B");

        public void Attach(IModHost modHost, IList<LightViewModel> lights)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Suspend()
        {
            throw new NotImplementedException();
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        public bool IsActive { get; }
        public IEnumerable<PluginSetting> Settings { get => settings; }

        public event IsActiveChangedEventHandler IsActiveChanged;
    }
}
