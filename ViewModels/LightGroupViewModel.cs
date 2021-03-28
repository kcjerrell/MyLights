using MyLights.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.ViewModels
{
    internal class LightGroupViewModel : LightViewModel
    {
        public LightGroupViewModel(LightGroup lightGroup) : base(lightGroup)
        {
        }

        public LightGroup LightGroup { get; } = new LightGroup();
    }
}
