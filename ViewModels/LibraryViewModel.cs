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
                Scenes.Add(DesignerHelpers.GetScene(5, false, true));
                Scenes.Add(DesignerHelpers.GetScene(5, true, false));
                Scenes.Add(DesignerHelpers.GetScene(5, false, false));

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

                App.Current.Exit += (s, e) => SaveLibrary();
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

        internal void RemoveScene(Scene scene)
        {
            if (Scenes.Remove(scene))
                SaveScenes();
        }

        private void SaveColors()
        {
            string favColors = JsonConvert.SerializeObject(FavColors.ToArray());
            Properties.Settings.Default.FavColors = favColors;
            Properties.Settings.Default.Save();
        }

        private void SaveScenes()
        {
            string scenes = JsonConvert.SerializeObject(Scenes.ToArray());
            Properties.Settings.Default.Scenes = scenes;
            Properties.Settings.Default.Save();
        }

        public void SaveLibrary()
        {
            SaveScenes();
            SaveColors();
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
