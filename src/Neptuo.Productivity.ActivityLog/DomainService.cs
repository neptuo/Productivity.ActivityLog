using Neptuo.Events;
using Neptuo.Productivity.ActivityLog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog
{
    public partial class DomainService : DisposableBase
    {
        private readonly IEventDispatcher eventDispatcher;
        private readonly ProcessMonitor monitor;

        public DomainService(IEventDispatcher eventDispatcher)
        {
            Ensure.NotNull(eventDispatcher, "eventDispatcher");
            this.eventDispatcher = eventDispatcher;

            monitor = new ProcessMonitor();
            monitor.Changed += OnProcessChanged;
            monitor.Start();
        }

        private async void OnProcessChanged(object sender, ProcessChangedEventArgs e)
        {
            DateTime now = DateTime.Now;    
            if (e.Type == ProcessChangedType.Process)
            {
                if (e.OriginalProcessId != null && e.OriginalPath != null)
                    await eventDispatcher.PublishAsync(new ActivityEnded(e.OriginalPath, e.OriginalTitle, now));

                if (e.CurrentPath != null)
                    await eventDispatcher.PublishAsync(new ActivityStarted(e.CurrentPath, e.CurrentTitle, now));
            }
            else if (e.Type == ProcessChangedType.Title)
            {
                if (e.OriginalPath != null)
                    await eventDispatcher.PublishAsync(new ActivityEnded(e.OriginalPath, e.OriginalTitle, now));

                if (e.CurrentPath != null)
                    await eventDispatcher.PublishAsync(new ActivityStarted(e.CurrentPath, e.CurrentTitle, now));
            }
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            monitor.Dispose();
        }
    }
}
