using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyLights.ViewModels
{
    public class StateSetViewModel : LibraryItemViewModel
    {
        public StateSetViewModel(RelayCommand<LibraryItemViewModel> editCommand)
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
                item.FindBulb();
                item.Set();
            }
        }

        internal override void StartEditing()
        {
            foreach (var item in SceneItems)
            {
                item.FindBulb();
                item.Set();
                item.IsLive = true;
            }
        }

        public class NewItem : StateSetViewModel
        {
            public NewItem(RelayCommand<LibraryItemViewModel> editCommand)
                : base(editCommand)
            {

            }
        }
    }
}