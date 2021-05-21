using MyLights.Models;
using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyLights.Windows.ViewModels
{
    public class SequenceEditorViewModel : INotifyPropertyChanged
    {
        public SequenceEditorViewModel()
        {
            for (int i = 0; i < 8; i++)
            {
                Sequence.Add(new());
            }

            StateA = new LightState()
            {
                Color = new HSV(0, 1, .8),
                Power = true,
                Mode = LightMode.Color
            };

            StateB = new LightState()
            {
                Color = new HSV(.2, 1, 1),
                Power = true,
                Mode = LightMode.Color
            };

            StartCommand = new RelayCommand((_) => Start());
            StopCommand = new RelayCommand((_) => Stop());
        }

        public LightViewModel TestLight { get; set; }

        public LightState StateA { get; set; }
        public LightState StateB { get; set; }

        public int Interval { get; set; } = 100;

        public ObservableCollection<SequenceStep> Sequence { get; } = new();

        public RelayCommand StartCommand { get; init; }
        public RelayCommand StopCommand { get; init; }

        Task sequenceTask;
        CancellationTokenSource ctSource;
        CancellationToken cToken;

        private async Task RunSequence(CancellationToken token)
        {
            int step = 0;
            LightState lastState = null;
            while (!token.IsCancellationRequested)
            {
                var state = Sequence[step].StateA ? StateA : StateB;
                if (state != lastState)
                    state.Apply(TestLight.Light);
                lastState = state;

                step += 1;
                if (step >= Sequence.Count)
                    step = 0;

                await Task.Delay(Interval);
            }

        }

        public void Start()
        {
            ctSource = new CancellationTokenSource();
            sequenceTask = RunSequence(ctSource.Token).ContinueWith((task) =>
            {
                ctSource.Dispose();
                ctSource = null;
            });
        }

        public void Stop()
        {
            ctSource.Cancel();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class SequenceStep : INotifyPropertyChanged
    {
        public SequenceStep()
        {
        }

        public bool StateA { get; set; } = false;
        public bool StateB { get => !StateA; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
