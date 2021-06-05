using MyLights.Models;
using MyLights.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.ViewModels
{
    /// <summary>
    /// Watches an ObservableCollection of LightViewModels for changes and synchronizes changes between
    /// LightViewModel's with IsLinked set.
    /// </summary>
    public class LightViewModelLinker
    {
        public LightViewModelLinker(ObservableCollection<LightViewModel> source)
        {
            this.SourceLights = source;
            SourceLights.CollectionChanged += WatchLights_CollectionChanged;

            foreach (LightViewModel lvm in SourceLights)
            {
                lvm.PropertyChanged += Lvm_PropertyChanged;
            }
        }

        private void Lvm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            LightViewModel source = (LightViewModel)sender;

            if (source.IsLinked)
            {
                Action<LightViewModel> linkedSet = e.PropertyName switch
                {
                    nameof(LightViewModel.Power) => (LightViewModel target) => target.Power = source.Power,
                    nameof(LightViewModel.Mode) => (LightViewModel target) => target.Mode = source.Mode,
                    nameof(LightViewModel.Brightness) => (LightViewModel target) => target.Brightness = source.Brightness,
                    nameof(LightViewModel.ColorTemp) => (LightViewModel target) => target.ColorTemp = source.ColorTemp,
                    nameof(LightViewModel.Color) => (LightViewModel target) => target.Color = source.Color,
                    _ => null
                };

                if (linkedSet == null)
                    return;

                var linked = from ll in SourceLights.Without(source)
                             where ll.IsLinked
                             select ll;

                foreach (var ll in linked)
                {
                    linkedSet(ll);
                }
            }
        }

        private void WatchLights_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (LightViewModel lvm in e.NewItems)
                {
                    lvm.PropertyChanged += Lvm_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (LightViewModel lvm in e.OldItems)
                {
                    lvm.PropertyChanged -= Lvm_PropertyChanged;
                }
            }
        }

        public ObservableCollection<LightViewModel> SourceLights { get; init; }

    }
}
