using System;
using System.Windows.Media;

namespace MyLights.ViewModels
{
    public class SequenceViewModel : LibraryItemViewModel
    {
        internal override void StartEditing()
        {
            throw new NotImplementedException();
        }

        public class NewItem : SequenceViewModel
        {

        }

    }
}