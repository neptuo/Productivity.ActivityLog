using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog
{
    /// <summary>
    /// A monitor which raises <see cref="Changed"/> everytime a foreground window or its title changes.
    /// </summary>
    public class ProcessMonitor : DisposableBase
    {
        private CancellationTokenSource cancellationSource;

        public event EventHandler<ProcessChangedEventArgs> Changed;

        public void Start()
        {
            if (cancellationSource != null)
                throw Ensure.Exception.InvalidOperation($"The '{nameof(ProcessMonitor)}' is already running.");

            cancellationSource = new CancellationTokenSource();
            Task.Factory.StartNew(DoMonitor);
        }

        private async void DoMonitor()
        {
            uint lastProcessId = 0;
            IntPtr lastHandle = IntPtr.Zero;
            string lastTitle = string.Empty;
            while (true)
            {
                await Task.Delay(1000);

                if (cancellationSource.Token.IsCancellationRequested)
                    break;

                // Checking process.
                IntPtr currentHandle = Win32.GetForegroundWindow();
                if (currentHandle != lastHandle)
                {
                    Win32.GetWindowThreadProcessId(currentHandle, out uint currentProcessId);

                    if (currentProcessId != lastProcessId)
                    {
                        // Process changed.
                        string title = Win32.GetWindowText(currentHandle);
                        Changed?.Invoke(this, new ProcessChangedEventArgs((int)lastProcessId, (int)currentProcessId, lastTitle, title));

                        lastHandle = currentHandle;
                        lastProcessId = currentProcessId;
                        lastTitle = title;
                        continue;
                    }
                }

                // Checking title.
                string currentTitle = Win32.GetWindowText(currentHandle);
                if (currentTitle != lastTitle)
                {
                    // Title changed.
                    Win32.GetWindowThreadProcessId(currentHandle, out uint currentProcessId);
                    Changed?.Invoke(this, new ProcessChangedEventArgs((int)currentProcessId, lastTitle, currentTitle));

                    lastTitle = currentTitle;
                    continue;
                }
            }
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            if (cancellationSource != null)
                cancellationSource.Cancel();
        }
    }
}
