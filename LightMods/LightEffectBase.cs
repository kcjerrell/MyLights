using MyLights.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyLights.LightMods
{
    public abstract class LightEffectBase : ILightEffect
    {
        protected IList<LightViewModel> Lights { get; private set; }
        protected IModHost ModHost { get; private set; }


        public virtual void Attach(IModHost modHost, IList<LightViewModel> lights)
        {
            this.Lights = lights;
            this.ModHost = modHost;
        }

        private CancellationTokenSource cancelSource;
        private Task procTask;

        protected virtual async Task ProcLoop(CancellationToken token)
        {

        }

        public virtual void Start()
        {
            IsActive = true;
            cancelSource = new CancellationTokenSource();
            procTask = ProcLoop(cancelSource.Token).ContinueWith((task) =>
            {
                IsActive = false;
                cancelSource.Dispose();
                cancelSource = null;
            });
        }

        public virtual void Suspend()
        {
            cancelSource?.Cancel();
        }

        public virtual void Shutdown()
        {
            cancelSource?.Cancel();
        }

        private bool _isActive = false;
        public bool IsActive
        {
            get => _isActive;
            protected set
            {
                _isActive = value;
                var handler = IsActiveChanged;
                handler?.Invoke(this,  IsActiveChangedEventArgs.GetStatic(_isActive));
            }
        }

        public new ObservableCollection<PluginSetting> Settings { get; } = new();

        public event IsActiveChangedEventHandler IsActiveChanged;

        IEnumerable<PluginSetting> ILightEffect.Settings { get => Settings; } 
    }
}
