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
        private readonly Dictionary<int, ProcessInfo> processCache = new Dictionary<int, ProcessInfo>();

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
                if (e.OriginalProcessId != null)
                {
                    if (TryGetInfo(e.OriginalProcessId.Value, out ProcessInfo originalInfo))
                        await eventDispatcher.PublishAsync(new ActivityEnded(originalInfo.Path, e.OriginalTitle, now));
                }

                if (TryGetInfo(e.CurrentProcessId, out ProcessInfo currentInfo))
                    await eventDispatcher.PublishAsync(new ActivityStarted(currentInfo.Path, e.CurrentTitle, now));
            }
            else if (e.Type == ProcessChangedType.Title)
            {
                if (TryGetInfo(e.CurrentProcessId, out ProcessInfo info))
                {
                    await eventDispatcher.PublishAsync(new ActivityEnded(info.Path, e.OriginalTitle, now));
                    await eventDispatcher.PublishAsync(new ActivityStarted(info.Path, e.CurrentTitle, now));
                }
            }
        }

        private bool TryGetInfo(int processId, out ProcessInfo info)
        {
            if (processCache.TryGetValue(processId, out info))
                return true;

            string path = null;
            try
            {
                Process process = Process.GetProcessById(processId);
                path = process.MainModule?.FileName;
            }
            catch (Exception)
            {
                return false;
            }

            processCache[processId] = info = new ProcessInfo
            {
                Path = path
            };
            return true;
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            monitor.Dispose();
        }
    }
}
