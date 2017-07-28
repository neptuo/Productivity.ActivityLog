using Neptuo.Productivity.ActivityLog.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemTimer = System.Timers.Timer;
using ElapsedEventArgs = System.Timers.ElapsedEventArgs;

namespace Neptuo.Productivity.ActivityLog
{
    public class Timer : DisposableBase, ITimer
    {
        private SystemTimer timer;

        public event Action Tick;

        public Timer()
        {
            timer = new SystemTimer(1000);
            timer.Elapsed += OnTimerElapsed;
            timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (Tick != null)
                Tick();
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            timer.Dispose();
        }
    }
}
