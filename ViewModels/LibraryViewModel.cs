using MyLights.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyLights.ViewModels
{
    public class LibraryViewModel : INotifyPropertyChanged
    {
        public LibraryViewModel()
        {

            SelectCommand = new RelayCommand<LibraryItemViewModel>(vm => vm.Activate());
            EditCommand = new RelayCommand<LibraryItemViewModel>(vm => Edit(vm));
            AddSceneCommand = new RelayCommand<LibraryItemViewModel>((vm) => AddScene());

            LoadDesignLibrary();
        }

        private void AddScene()
        {
            var scene = new SceneViewModel(EditCommand);
            Scenes.Add(scene);
            Edit(scene);
        }

        private void Edit(LibraryItemViewModel vm)
        {
            EditorItem = vm;
            ShowEditor = true;
        }

        private void LoadDesignLibrary()
        {
            var sceneNew = new SceneViewModel.NewItem(AddSceneCommand);
            var sceneA = new SceneViewModel(EditCommand)
            {
                Color = Colors.Pink,
                Name = "SexyTime",
                DateCreated = DateTime.Now - TimeSpan.FromDays(7),
                DateUpdated = DateTime.Now - TimeSpan.FromDays(1)
            };

            var br = new BulbRef() { Name = "UpRight" };
            sceneA.Add(new SceneSetter(br, true, new HSV(0.5, 0.8, 1)));
            br = new BulbRef() { Name = "Face" };
            sceneA.Add(new SceneSetter(br, true, new HSV(1, 0.8, 1)));
            br = new BulbRef() { Name = "Up" };
            sceneA.Add(new SceneSetter(br, true, new HSV(0.7, 1, 1)));

            var sceneB = new SceneViewModel(EditCommand)
            {
                Color = Colors.Blue,
                Name = "Movie Nite",
                DateCreated = DateTime.Now - TimeSpan.FromDays(4),
                DateUpdated = DateTime.Now - TimeSpan.FromDays(4)
            };
            var sceneC = new SceneViewModel(EditCommand)
            {
                Color = Colors.White,
                Name = "Reading",
                DateCreated = DateTime.Now - TimeSpan.FromDays(8),
                DateUpdated = DateTime.Now - TimeSpan.FromDays(6)
            };

            scenes = new List<SceneViewModel>() { sceneA, sceneB, sceneC, sceneNew };

            Scenes = new ObservableCollection<SceneViewModel>(scenes);

            var sequenceNew = new SequenceViewModel.NewItem();
            var sequenceA = new SequenceViewModel()
            {
                Color = Colors.Red,
                Name = "Striptease",
                DateCreated = DateTime.Now - TimeSpan.FromDays(7),
                DateUpdated = DateTime.Now - TimeSpan.FromDays(1)
            };
            var sequenceB = new SequenceViewModel()
            {
                Color = Colors.Green,
                Name = "Trippy",
                DateCreated = DateTime.Now - TimeSpan.FromDays(2),
                DateUpdated = DateTime.Now - TimeSpan.FromDays(2)
            };
            var sequenceC = new SequenceViewModel()
            {
                Color = Colors.Gray,
                Name = "Alarm",
                DateCreated = DateTime.Now - TimeSpan.FromDays(5),
                DateUpdated = DateTime.Now - TimeSpan.FromDays(3)
            };

            sequences = new List<SequenceViewModel>() { sequenceA, sequenceB, sequenceC,
                                                       sequenceNew};
            Sequences = new ObservableCollection<SequenceViewModel>(sequences);

        }

        private List<SceneViewModel> scenes;
        private List<SequenceViewModel> sequences;

        public bool ShowEditor { get; set; }
        public LibraryItemViewModel EditorItem { get; set; }

        public ObservableCollection<SequenceViewModel> Sequences { get; private set; }
        public ObservableCollection<SceneViewModel> Scenes { get; private set; }

        public RelayCommand<LibraryItemViewModel> SelectCommand { get; }
        public RelayCommand<LibraryItemViewModel> EditCommand { get; }
        public RelayCommand<LibraryItemViewModel> DeleteCommand { get; }
        public RelayCommand<LibraryItemViewModel> AddSceneCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
