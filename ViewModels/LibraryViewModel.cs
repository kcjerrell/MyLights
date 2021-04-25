using MyLights.Models;
using MyLights.Util;
using Newtonsoft.Json;
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
            SaveStateAsSceneCommand = new RelayCommand(_ => SaveStateAsScene());
        }

        private void SaveStateAsScene()
        {
            var lvms = Locator.Get.LightVMs;
            var scene = new Scene("New Scene");

            foreach (var lvm in lvms)
            {
                scene.Add(lvm.Light);
            }

            Scenes.Add(scene);

            SaveLibrary();
        }


        private void AddScene()
        {
            var scene = new SceneViewModel(EditCommand);
            //Scenes.Add(scene);
            Edit(scene);
        }

        private void Edit(LibraryItemViewModel vm)
        {
            EditorItem = vm;
            ShowEditor = true;
            vm.StartEditing();
        }

        public void LoadLibrary(bool isInDesignMode)
        {
            // Properties.Settings.Default.Reset();                    
            if (isInDesignMode)
            {
                Scenes.Add(new Scene("Sample Scene #1"));
                Scenes.Add(new Scene("Sample Scene #2"));

                FavColors.Add(new HSV(1, 1, 1));
                FavColors.Add(new HSV(.3, .8, 1));
            }
            else
            {
                try
                {
                    string scenes = Properties.Settings.Default.Scenes;
                    var array = JsonConvert.DeserializeObject<Scene[]>(scenes);

                    foreach (var s in array ?? Array.Empty<Scene>())
                    {
                        Scenes.Add(s);
                    }
                }
                catch (Exception)
                {
                }

                var favColors = JsonConvert.DeserializeObject<HSV[]>(Properties.Settings.Default.FavColors);

                foreach (var c in favColors ?? Array.Empty<HSV>())
                {
                    FavColors.Add(c);
                }

                if (FavColors.Count == 0)
                {
                    FavColors.Add(new HSV(1, 1, 1));
                    FavColors.Add(new HSV(.3, .8, 1));

                    SaveLibrary();
                }
            }
        }

        internal void RemoveColor(HSV color)
        {
            if (FavColors.Remove(color))
            {
                SaveColors();
            }
        }

        public void AddColor(HSV color)
        {
            if (!FavColors.Contains(color))
            {
                FavColors.Add(color);
                SaveColors();
            }

        }

        private void SaveColors()
        {
            string favColors = JsonConvert.SerializeObject(FavColors.ToArray());
            Properties.Settings.Default.FavColors = favColors;
        }

        public void SaveLibrary()
        {
            string scenes = JsonConvert.SerializeObject(Scenes.ToArray());
            Properties.Settings.Default.Scenes = scenes;
            Properties.Settings.Default.Save();

            SaveColors();
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
            sceneA.Add(new SceneSetter(br) { Power = true, Color = new HSV(0.5, 0.8, 1), Mode = "color" });
            br = new BulbRef() { Name = "Face" };
            sceneA.Add(new SceneSetter(br) { Power = true, Color = new HSV(1, 0.8, 1), Mode = "color" });
            br = new BulbRef() { Name = "Up" };
            sceneA.Add(new SceneSetter(br) { Power = true, Color = new HSV(0.7, 1, 1), Mode = "color" });

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

            // scenes = new List<SceneViewModel>() { sceneA, sceneB, sceneC, sceneNew };

            // Scenes = new ObservableCollection<SceneViewModel>(scenes);

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

            // sequences = new List<SequenceViewModel>() { sequenceA, sequenceB, sequenceC,
            //                                           sequenceNew};
            // Sequences = new ObservableCollection<SequenceViewModel>(sequences);

        }

        internal void ApplyScene(Scene scene)
        {
            foreach ((string bulbName, LightState state) in scene.LightStates)
            {
                if (Locator.Get.LightBridge.TryFindBulb(new BulbRef() { Name = bulbName }, out Light light))
                {
                    state.Apply(light);
                }
            }
        }

        // private List<SceneViewModel> scenes;
        // private List<SequenceViewModel> sequences;

        public bool ShowEditor { get; set; }
        public LibraryItemViewModel EditorItem { get; set; }

        public ObservableCollection<SequenceViewModel> Sequences { get; private set; } = new();
        public ObservableCollection<Scene> Scenes { get; private set; } = new();

        public ObservableCollection<HSV> FavColors { get; private set; } = new();

        public RelayCommand<LibraryItemViewModel> SelectCommand { get; }
        public RelayCommand<LibraryItemViewModel> EditCommand { get; }
        public RelayCommand<LibraryItemViewModel> DeleteCommand { get; }
        public RelayCommand<LibraryItemViewModel> AddSceneCommand { get; }

        public RelayCommand SaveStateAsSceneCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
