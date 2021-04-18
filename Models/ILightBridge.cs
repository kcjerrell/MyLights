using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Models
{
    public interface ILightBridge
    {
        public ObservableCollection<Light> Lights { get; }
        public ObservableCollection<LightViewModel> LightVMs { get; }

        public bool TryFindBulb(BulbRef key, out Light light);
    }
}
