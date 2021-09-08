using MyLights.Models;
using MyLights.Util;
using MyLights.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Bridges
{
    class TestLightBridge : ILightBridge
    {
        public ObservableCollection<Light> Lights => lights;

        public ObservableCollection<LightViewModel> LightVMs => lightVMs;

        public bool TryFindBulb(BulbRef key, out Light light)
        {
            light = null;
            return false;
        }

        public async Task ConnectAsync()
        {
            await Task.Delay(10);
        }

        static TestLightBridge()
        {
            for (int i = 0; i < 3; i++)
            {
                var props = new TestPropertiesProvider();
                var light = new Light(props);

                lights.Add(light);

                lightVMs.Add(new LightViewModel(light));
            }
        }

        static ObservableCollection<Light> lights = new();

        static ObservableCollection<LightViewModel> lightVMs = new();

        public void Reload()
        {
            throw new System.NotImplementedException();
        }
    }
}
