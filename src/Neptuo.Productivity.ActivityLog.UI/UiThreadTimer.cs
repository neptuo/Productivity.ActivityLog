using Neptuo;
using Neptuo.Productivity.ActivityLog.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Neptuo.Productivity.ActivityLog
{
    internal class UiThreadTimer : DisposableBase, ITimer
    {
        private Timer timer;
        private ISynchronizer synchronizer;

        public event Action Tick;

        public UiThreadTimer(ISynchronizer synchronizer)
        {
            Ensure.NotNull(synchronizer, "synchronizer");
            this.synchronizer = synchronizer;

            timer = new Timer(1000);
            timer.Elapsed += OnTimerElapsed;
            timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (Tick != null)
            {
                synchronizer.Run(() =>
                {
                    if (Tick != null)
                        Tick();
                });
            }
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            timer.Dispose();
        }
    }
}
