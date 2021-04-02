using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLights.Patcher.ViewModels
{
    public abstract class Module : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public double Left { get; set; }
        public double Top { get; set; }

        public IReadOnlyList<Input> Inputs { get; private set; } = new List<Input>();
        public IReadOnlyList<Output> Outputs { get; private set; } = new List<Output>();

        protected void RegisterInput(Input input)
        {

        }

        protected void RegisterOutput(Output output)
        {

        }
    }


    public class Modulator : Module
    {
        public Modulator()
        {
            RegisterInput(inX);
            RegisterOutput(outY);
        }

        Input<double> inX;
        Output<double> outY;
    }

    public class LightModule : Module
    {
    
    }

    public class Input
    { }


    public class Input<T> : Input
    {

    }

    public class Output
    { }


    public class Output<T> : Output
    {

    }

}
