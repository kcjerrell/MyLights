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
    public class ModHost : INotifyPropertyChanged
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
            handler?.Invoke(this, new PropertyChangedEventArgs("DeviceEffectsPlugin"));
        }

        public ObservableCollection<ILightPlugin> Plugins { get; } = new ObservableCollection<ILightPlugin>();
        public IEnumerable<ILightPlugin> DeviceEffectsPlugins
        {
            get
            {
                return from p in Plugins
                       where (p.Properties & PluginProperties.DeviceEffect) == PluginProperties.DeviceEffect
                       select p;
            }
        }

        internal async Task Load(bool isInDesignMode)
        {
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
                                return;
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
}
