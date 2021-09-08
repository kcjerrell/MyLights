using MyLights.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.ViewModels
{
    public class SceneViewModel : INotifyPropertyChanged
    {
        public SceneViewModel(Scene scene)
        {

        }

        ObservableCollection<SceneStop> Stops { get; } = new();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
