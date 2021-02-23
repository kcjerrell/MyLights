using System.ComponentModel;

namespace MyLights.ViewModels
{
    internal class LightTrackViewModel : INotifyPropertyChanged
    {
        public LightTrackViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}