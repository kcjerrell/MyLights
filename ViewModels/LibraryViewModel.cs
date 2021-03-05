﻿using MyLights.Models;
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
            LoadDesignLibrary();

            SelectCommand = new RelayCommand<LibraryItemViewModel>(vm => vm.Activate());
            EditCommand = new RelayCommand<LibraryItemViewModel>(vm => Edit(vm));
        }

        private void Edit(LibraryItemViewModel vm)
        {
            
        }

        private void LoadDesignLibrary()
        {
            var sceneNew = new SceneViewModel.NewItem();
            var sceneA = new SceneViewModel()
            {
                Color = Colors.Pink,
                Name = "SexyTime",
                DateCreated = DateTime.Now - TimeSpan.FromDays(7),
                DateUpdated = DateTime.Now - TimeSpan.FromDays(1)
            };

            var br = new BulbRef() { Name = "UpRight" };
            sceneA.Add(new ColorSetter(br, new HSV(0.5, 0.8, 1)));
            sceneA.Add(new PowerSetter(br, true));
            br = new BulbRef() { Name = "Face" };
            sceneA.Add(new ColorSetter(br, new HSV(1, 0.8, 1)));
            sceneA.Add(new PowerSetter(br, true));
            br = new BulbRef() { Name = "Up" };
            sceneA.Add(new ColorSetter(br, new HSV(0.7, 1, 1)));
            sceneA.Add(new PowerSetter(br, true));

            var sceneB = new SceneViewModel()
            {
                Color = Colors.Blue,
                Name = "Movie Nite",
                DateCreated = DateTime.Now - TimeSpan.FromDays(4),
                DateUpdated = DateTime.Now - TimeSpan.FromDays(4)
            };
            var sceneC = new SceneViewModel()
            {
                Color = Colors.White,
                Name = "Reading",
                DateCreated = DateTime.Now - TimeSpan.FromDays(8),
                DateUpdated = DateTime.Now - TimeSpan.FromDays(6)
            };

            scenes = new List<SceneViewModel>() { sceneA, sceneB, sceneC, sceneNew,
                                                        };

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

        public ObservableCollection<SequenceViewModel> Sequences { get; private set; }
        public ObservableCollection<SceneViewModel> Scenes { get; private set; }

        public RelayCommand<LibraryItemViewModel> SelectCommand { get; }
        public RelayCommand<LibraryItemViewModel> EditCommand { get; }
        public RelayCommand<LibraryItemViewModel> DeleteCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}