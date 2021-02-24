﻿using System;
using System.ComponentModel;
using System.Windows.Media;

namespace MyLights.ViewModels
{
    public class LibraryItemViewModel : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public DateTime DateUpdated { get; set; }
        public DateTime DateCreated { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}