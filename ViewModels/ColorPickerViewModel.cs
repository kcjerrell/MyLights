using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyLights.ViewModels
{
    public class ColorPickerViewModel : INotifyPropertyChanged
    {
        public Color OldColor { get; set; }
        public Color SelectedColor { get; set; }
        public Color SelectedShade { get; set; }
        public double ValueValue { get; set; }
        public bool ShowOldColor { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
