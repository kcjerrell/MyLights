using MyLights.LightMods;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyLights.Util
{
    public class ModHost
    {
        internal async Task Load(bool isInDesignMode)
        {
            if (isInDesignMode)
            {

            }
            else
            {
                string pluginPath = @"C:\Users\kcjer\source\repos\LightBridge\LightMods\bin\Debug\net5.0-windows\LightMods.dll";

                Assembly pluginAssembly = LoadPlugin(pluginPath);
                IEnumerable<ILightModCreator> mods = CreateMods(Assembly.GetExecutingAssembly());

                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    foreach (ILightModCreator mod in mods)
                    {
                        LightMods.Add(mod);
                    }
                });
            }
        }

        public ObservableCollection<ILightModCreator> LightMods { get; } = new ObservableCollection<ILightModCreator>();

        Assembly LoadPlugin(string relativePath)
        {
            // Navigate up to the solution root
            string root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(typeof(App).Assembly.Location)))))));

            string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));

            Console.WriteLine($"Loading mods from: {pluginLocation}");
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }

        private IEnumerable<ILightModCreator> CreateMods(Assembly assembly)
        {
            int count = 0;
            var types = assembly.GetTypes();
            foreach (Type type in types)
            {
                Console.WriteLine(type);
                if (typeof(ILightModCreator).IsAssignableFrom(type))
                {
                    ILightModCreator result = Activator.CreateInstance(type) as ILightModCreator;
                    if (result != null)
                    {
                        count++;
                        yield return result;
                    }
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements ICommand in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }
    }
}