using MyLights.Models;
using System;
using System.Collections.Generic;

namespace MyLights.ViewModels
{
    public class SceneViewModel : LibraryItemViewModel
    {
        public SceneViewModel()
        {

        }

        List<SceneSetter> sceneItems = new List<SceneSetter>();

        public void Add(SceneSetter item)
        {
            sceneItems.Add(item);
        }

        internal override void Activate()
        {
            //App.Current.Locator.MainWindowViewModel.RightViewModel = this;
            foreach (var item in sceneItems)
            {
                item.Set();
            }
        }

        public class NewItem : SceneViewModel
        {

        }
    }

    public abstract class SceneSetter
    {        
        public void Set()
        {
            var lightBridge = App.Current.Locator.LightBridge;

            if (lightBridge.TryFindBulb(BulbRef, out Light light))
            {
                SetOverride(lightBridge.GetLightViewModel(light));
            }
        }

        public BulbRef BulbRef { get; set; }

        public abstract void SetOverride(LightViewModel lightVM);
    }

    public class PowerSetter : SceneSetter
    {
        public PowerSetter(BulbRef bulbRef, bool power)
        {
            this.BulbRef = bulbRef;
            this.power = power;
        }

        bool power;

        public override void SetOverride(LightViewModel lightVM)
        {
            lightVM.Power = power;
        }
    }

    public class ColorSetter : SceneSetter
    {
        public ColorSetter(BulbRef bulbRef, HSV color)
        {
            this.BulbRef = bulbRef;
            this.color = color;
        }

        HSV color;

        public override void SetOverride(LightViewModel lightVM)
        {
            lightVM.HSV = color;
        }

    }
}