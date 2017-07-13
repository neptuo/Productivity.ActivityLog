using Neptuo.Events;
using Neptuo.Productivity.ActivityLog.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                    ProcessInfo originalInfo = GetInfo(e.OriginalProcessId.Value);
                    await eventDispatcher.PublishAsync(new ActivityEnded(originalInfo.Path, e.OriginalTitle, now));
                }

                ProcessInfo currentInfo = GetInfo(e.CurrentProcessId);
                await eventDispatcher.PublishAsync(new ActivityStarted(currentInfo.Path, e.CurrentTitle, now));
            }
            else if (e.Type == ProcessChangedType.Title)
            {
                ProcessInfo info = GetInfo(e.CurrentProcessId);
                await eventDispatcher.PublishAsync(new ActivityEnded(info.Path, e.OriginalTitle, now));
                await eventDispatcher.PublishAsync(new ActivityStarted(info.Path, e.CurrentTitle, now));
            }
        }

        private ProcessInfo GetInfo(int processId)
        {
            if (!processCache.TryGetValue(processId, out ProcessInfo info))
            {
                string path = null;
                try
                {
                    Process process = Process.GetProcessById(processId);
                    path = process.MainModule?.FileName;
                    
                }
                catch(Win32Exception e)
                {
                    StringBuilder builder = new StringBuilder(1024);
                    Win32.GetProcessImageFileName(new IntPtr(processId), builder, builder.Capacity);

                    path = builder.ToString();
                }

                processCache[processId] = info = new ProcessInfo
                {
                    Path = path
                };
            }

            return info;
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            monitor.Dispose();
        }
    }
}
