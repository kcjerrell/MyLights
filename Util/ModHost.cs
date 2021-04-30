using MyLights.LightMods;
using MyLights.ViewModels;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Util
{
    public class ModHost : IModHost, INotifyPropertyChanged
    {
        public ModHost(bool isInDesignMode)
        {
            if (isInDesignMode)
            {
                Plugins.Add(new Blinker());
            }

            Plugins.CollectionChanged += Plugins_CollectionChanged;
        }

        private void Plugins_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(nameof(DeviceEffectsPlugins)));
            handler?.Invoke(this, new PropertyChangedEventArgs(nameof(GlobalModPlugins)));
        }

        public ObservableCollection<ILightPlugin> Plugins { get; } = new ObservableCollection<ILightPlugin>();
        public ReadOnlyObservableCollection<LightViewModel> LightViewModels { get; private set; }
        public IEnumerable<ILightPlugin> DeviceEffectsPlugins
        {
            get
            {
                return from p in Plugins
                       where (p.Properties & PluginProperties.DeviceEffect) == PluginProperties.DeviceEffect
                       select p;
            }
        }
        public IEnumerable<ILightPlugin> GlobalModPlugins
        {
            get
            {
                return from p in Plugins
                       where (p.Properties & PluginProperties.GlobalMod) == PluginProperties.GlobalMod
                       select p;
            }
        }

        internal async Task Load(bool isInDesignMode)
        {
            LightViewModels = new ReadOnlyObservableCollection<LightViewModel>(Locator.Get.LightVMs);

            if (!isInDesignMode)
            {
                //Assembly.LoadFrom(@"C:\Users\kcjer\source\repos\LightBridge\LightMods\bin\Debug\net5.0-windows\LightMods.dll");

                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type t in a.GetTypes())
                    {
                        if (t.GetInterface("ILightPlugin") != null)
                        {
                            try
                            {
                                ILightPlugin pluginclass = Activator.CreateInstance(t) as ILightPlugin;
                                Plugins.Add(pluginclass);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }

        internal IDeviceEffect Create(ILightPlugin mc, LightViewModel viewModel)
        {
            var effect = mc.GetDeviceMod(viewModel);
            return effect;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public interface IModHost
    {
        public ReadOnlyObservableCollection<LightViewModel> LightViewModels { get; }
    }
}
