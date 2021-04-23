using MyLights.Util;
using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using PropertyChanged;

namespace MyLights.Windows.ViewModels
{
    public class Modulator : INotifyPropertyChanged
    {
        public const int UpdateInterval = 200;

        public Modulator()
        {
            Function = (x) =>
            {
                // I know there's a way to do this simply in one expression but
                // the performance impact of me not knowing, in this case, will be none.
                if (x < 0.5)
                    return x * 2.0;
                else
                    return 1.0 - 2 * (x - 0.5);
            };

        }

        private TimeSpan totalElapsed;

        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;

                if (_isActive)
                {
                    totalElapsed = TimeSpan.Zero;
                    JoinSync(this);
                }
                else
                    RemoveSync(this);
            }
        }

        public ObservableCollection<LightViewModel> Lights { get => lights; }
        public LightViewModel TargetLight { get; set; }
        public TargetProperties TargetProperty { get; set; }
        public double Value { get; set; } = 0.0;
        public double ValueMin { get; set; } = 0.0;
        public double ValueMax { get; set; } = 1.0;
        public double ValueRange { get => ValueMax - ValueMin; }
        public TimeSpan Period { get; set; } = TimeSpan.FromSeconds(10);
        [DependsOn("Period")]
        [AlsoNotifyFor("Period")]
        public double PeriodSeconds { get => Period.TotalSeconds; set => Period = TimeSpan.FromSeconds(value); }
        public TimeSpan PhaseOffset { get; set; } = TimeSpan.Zero;
        [DependsOn("PhaseOffset")]
        [AlsoNotifyFor("PhaseOffset")]
        public double PhaseOffsetSeconds { get => PhaseOffset.TotalSeconds; set => PhaseOffset = TimeSpan.FromSeconds(value); }

        /// <summary>
        /// This function should take a value from 0.0-1.0 and return a value from 0.0-1.0
        /// By default, this will be a "triangle" function
        /// 
        /// </summary>
        public Func<double, double> Function { get; set; }

        private void Proc(TimeSpan elapsed)
        {
            // I succesfully made a simple oscillator/synth in c# once, but it was many many years ago
            // and I don't remember a lot of it.
            // Although with that, you wouldn't calculate it's value with a formula like this, you would
            // have a value that you push up and down.
            // Because if you change any of it's parameters it will crack

            //I guess that applies here though too, because if I change my period from 10s to 20s, the 
            //light color/brightness might jump to a new value rather than just slowing down... hmmm.

            //I need to turn elapsed, period, and start, into a value between 0-1
            //actually

            totalElapsed += elapsed;

            double t = ((totalElapsed + PhaseOffset).TotalSeconds % Period.TotalSeconds) / Period.TotalSeconds;
            // <3 %

            double y = Function(t);

            if (y < 0.0 || y > 1.0)
            {
                throw new Exception("uhoh");
            }

            Value = y * ValueRange + ValueMin;

            UpdateLight();
        }

        private void UpdateLight()
        {
            if (TargetLight != null && TargetProperty != TargetProperties.None)
            {
                switch (TargetProperty)
                {
                    case TargetProperties.None:
                        break;
                    case TargetProperties.Hue:
                        TargetLight.H = Value;
                        break;
                    case TargetProperties.Saturation:
                        TargetLight.S = Value;
                        break;
                    case TargetProperties.Value:
                        TargetLight.V = Value;
                        break;
                    default:
                        break;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        static Modulator()
        {
            lights = Locator.Get.LightVMs;

            //I know this has to be on the correct thread. Actually, maybe it doesn't since it doesn't
            //directly touch the UI at all. we shall see
            timer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(UpdateInterval),
                IsEnabled = false,
            };

            timer.Tick += (s, e) => SyncProc();
        }

        private static List<Modulator> syncMods = new List<Modulator>();
        private static DispatcherTimer timer;
        private static DateTime lastProc;

        private static ObservableCollection<LightViewModel> lights;

        public static bool IsSyncActive { get => timer.IsEnabled; private set => timer.IsEnabled = value; }

        private static void SyncProc()
        {
            DateTime now = DateTime.Now;
            TimeSpan elapsed = now - lastProc;
            lastProc = now;

            foreach (var mod in syncMods)
            {
                mod.Proc(elapsed);
            }
        }

        private static void JoinSync(Modulator mod)
        {
            syncMods.Add(mod);

            if (!IsSyncActive)
            {
                lastProc = DateTime.Now;
                IsSyncActive = true;
            }
        }

        private static void RemoveSync(Modulator mod)
        {
            syncMods.Remove(mod);

            if (syncMods.Count == 0)
            {
                IsSyncActive = false;
            }
        }

        //private static void StartSync()
        //{
        //    if (!IsSyncActive)
        //    {
        //        lastProc = DateTime.Now;
        //        IsSyncActive = true;
        //    }
        //}
        //
        //private static void StopSync()
        //{
        //    if (IsSyncActive)
        //    {
        //        IsSyncActive = false;
        //    }
        //}
    }

    public enum TargetProperties
    {
        None,
        Hue,
        Saturation,
        Value
    }
}
