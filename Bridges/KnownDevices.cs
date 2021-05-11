using MyLights.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Bridges
{
    public static class KnownDevices
    {
        public static string GetName(string id)
        {
            var device = GetDevice(id);
            return device.Name;
        }

        private static ObservableCollection<DevicePreferences> devices;

        private static DevicePreferences GetDevice(string id)
        {
            DevicePreferences device;
            try
            {
                device = devices.Single(d => d.Id == id);
            }
            catch (InvalidOperationException)
            {
                device = new DevicePreferences();
                device.Id = id;
                device.Name = "Unnamed Device";
                devices.Add(device);
            }

            return device;
        }

        internal static void UpdateName(string id, string name)
        {
            var dev = GetDevice(id);
            if (dev.Name != name)
            {
                dev.Name = name;
                SaveDevices();
            }
        }

        static KnownDevices()
        {
            if (string.IsNullOrEmpty(Settings.Default.DevicePreferences))
                devices = new ObservableCollection<DevicePreferences>();
            else
                devices = JsonConvert.DeserializeObject<ObservableCollection<DevicePreferences>>(Settings.Default.DevicePreferences);

            devices.CollectionChanged += (s,e) => SaveDevices();
        }

        private static void SaveDevices()
        {
            Settings.Default.DevicePreferences = JsonConvert.SerializeObject(devices);
        }


        public class DevicePreferences
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}
