using MyLights.Util;
using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyLights.LightMods
{
    public class ModHost : IModHost, INotifyPropertyChanged
    {
        public ModHost(bool isInDesignMode)
        {
            LightViewModels = new(Locator.Get.LightVMs);
        }

        public List<LightEffectsInfo> SingleEffectTypes { get; } = new();
        public List<LightEffectsInfo> MultiEffectTypes { get; } = new();
        public ReadOnlyObservableCollection<LightViewModel> LightViewModels { get; }

        internal async Task Load(bool isInDesignMode)
        {
            if (!isInDesignMode)
            {
                //Assembly.LoadFrom(@"C:\Users\kcjer\source\repos\LightBridge\LightMods\bin\Debug\net5.0-windows\LightMods.dll");

                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var lfx = (from t in a.GetTypes()
                               where t.GetInterface(nameof(ILightEffect)) != null
                               select t).ToList();

                    var singlefx = from t in a.GetTypes()
                                   where t.GetInterface(nameof(ILightEffect)) != null
                                   where Attribute.IsDefined(t, typeof(LightEffectAttribute))
                                   select new LightEffectsInfo(t);

                    SingleEffectTypes.AddRange(singlefx);

                    var doublelefx = from t in a.GetTypes()
                                     where t.GetInterface(nameof(ILightEffect)) != null
                                     where Attribute.IsDefined(t, typeof(MultiLightEffectAttribute))
                                     select new LightEffectsInfo(t);

                    MultiEffectTypes.AddRange(doublelefx);
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class LightEffectsInfo
    {
        public LightEffectsInfo(Type type)
        {
            var attr = (LightEffectAttribute)Attribute.GetCustomAttribute(type, typeof(LightEffectAttribute));
            Name = attr.Name;
            ImageSource Icon = new BitmapImage(new Uri("/Puzzles-256.png", UriKind.Relative));
            this.Type = type;
        }

        public Type Type { get; set; }
        public string Name { get; set; }
        public BitmapSource Icon { get; set; }
        public ILightEffect Load()
        {
            try
            {
                var effect = Activator.CreateInstance(this.Type) as ILightEffect;             
                return effect;
            }

            catch (Exception ex) {
                throw new Exception("oops", ex);
            }
        }
    }
}
