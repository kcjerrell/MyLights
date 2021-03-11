using MyLights.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyLights.ViewModels
{
    public class SceneViewModel : LibraryItemViewModel
    {
        public SceneViewModel(RelayCommand<LibraryItemViewModel> editCommand)
        {
            this.EditCommand = editCommand;
            SceneItems = new ObservableCollection<SceneSetter>();
        }

        public RelayCommand<LibraryItemViewModel> EditCommand { get; }
        public ObservableCollection<SceneSetter> SceneItems { get; private set; }

        public void Add(SceneSetter item)
        {
            SceneItems.Add(item);
        }

        internal override void Activate()
        {
            foreach (var item in SceneItems)
            {
                item.Set();
            }
        }

        public class NewItem : SceneViewModel
        {
            public NewItem(RelayCommand<LibraryItemViewModel> editCommand)
                :base(editCommand)
            {

            }
        }
    }

    public class SceneSetter
    {
        public SceneSetter(BulbRef bulbRef, bool power, HSV color)
        {

        }

        public void Set()
        {
            var lightBridge = App.Current.Locator.LightBridge;
            if (lightBridge.TryFindBulb(BulbRef, out Light light))
            {
                light.SetColor(Color);
                light.SetPower(Power);
            }
        }

        public HSV Color { get; set; }
        public bool Power { get; set; }
        public BulbRef BulbRef { get; set; }
    }
}