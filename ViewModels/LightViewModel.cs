using MyLights.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyLights.ViewModels
{
    public class LightViewModel : INotifyPropertyChanged
    {
        private bool power;

        public LightViewModel(Light light)
        {
            TogglePowerCommand = new RelayCommand(s => TogglePower());

            this.light = light;

            Name = light.Name;
            Power = light.Power;
            Color = light.Color;
        }

        private void TogglePower()
        {
            throw new NotImplementedException();
        }

        public string Name { get; }
        public bool Power
        {
            get => power;
            set
            {
                power = value;
                light.SetPower(value);
            }
        }
        public Color Color 
        { 
            get => color;
            set
            {
                color = value;
                light.SetColor(value);
            }
        }

        public int Index { get => light.Index; }

        //using the vm should have everything abstracted away
        //lvm.Color = Color.FromRGB(25,,0,0);
        //lvm.Power = false;

        //and of course the vm should have no idea how the lights actually work except for
        //I don't think i'm going to use direct property accessors, since "getting" or "setting"
        //the value on a bulb /is/ an operation 
        //so using the model will resemeble:

        //l.SetColor(Color.FromRGB(255,0,0));          <-actually I might skip using the Color struct
        //                                               since it can just use parameters

        //Now I think I might have property GETTERS for power/color and just keep their current values
        //cached. again, on the model.

        //but wait! how am I going to combine commands across bulbs? I dunno yet. probably doing all this wrong



        public RelayCommand TogglePowerCommand { get; }

        private Light light;
        private Color color;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
