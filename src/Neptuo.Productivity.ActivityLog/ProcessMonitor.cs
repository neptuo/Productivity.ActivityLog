using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly Dictionary<int, string> processCache = new Dictionary<int, string>();
        private CancellationTokenSource cancellationSource;
        private readonly HashSet<string> hostingProcessNames = new HashSet<string>() { "ApplicationFrameHost" };

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
            string lastPath = string.Empty;
            string lastTitle = string.Empty;
            while (true)
            {
                await Task.Delay(1000);

                if (cancellationSource.Token.IsCancellationRequested)
                    break;

                // Checking process.
                IntPtr currentHandle = Win32.GetForegroundWindow();
                string currentTitle = Win32.GetWindowText(currentHandle);
                Win32.GetWindowThreadProcessId(currentHandle, out uint currentProcessId);
                if (currentHandle != lastHandle)
                {
                    if (currentProcessId != lastProcessId)
                    {
                        if (TryGetApplicationPath((int)currentProcessId, out string currentPath))
                        {
                            if (lastProcessId > 0)
                            {
                                // Process changed and we know last process.
                                Changed?.Invoke(this, ProcessChangedEventArgs.ForProcessChange(
                                    (int)lastProcessId,
                                    (int)currentProcessId,
                                    lastPath,
                                    currentPath,
                                    lastTitle,
                                    currentTitle
                                ));
                            }
                            else
                            {
                                Changed?.Invoke(this, ProcessChangedEventArgs.ForProcessChange(
                                    (int)currentProcessId,
                                    currentPath,
                                    currentTitle
                                ));
                            }
                        }
                        else if(lastProcessId > 0)
                        {
                            Changed?.Invoke(this, ProcessChangedEventArgs.ForProcessChange(
                                (int)lastProcessId,
                                (int)currentProcessId,
                                lastPath,
                                currentPath,
                                lastTitle,
                                currentTitle
                            ));
                        }

                        lastProcessId = currentProcessId;
                        lastPath = currentPath;
                        lastTitle = currentTitle;

                        continue;
                    }
                }

                // Checking title.
                if (currentTitle != lastTitle)
                {
                    if (TryGetApplicationPath((int)currentProcessId, out string currentPath))
                    {
                        // Title changed.
                        Changed?.Invoke(this, ProcessChangedEventArgs.ForTitleChange(
                            (int)currentProcessId, 
                            lastTitle, 
                            currentTitle
                        ));

                        lastTitle = currentTitle;
                        continue;
                    }
                }
            }
        }

        private bool TryGetApplicationPath(int processId, out string applicationPath)
        {
            if (processCache.TryGetValue(processId, out applicationPath))
                return true;

            try
            {
                Process process = Process.GetProcessById(processId);
                if (hostingProcessNames.Contains(process.ProcessName))
                {
                    Process child = Win32.FindChildProcess(process.MainWindowHandle, p => !hostingProcessNames.Contains(p.ProcessName));
                    if (child != null)
                        process = child;
                }

                processCache[processId] = applicationPath = process.MainModule?.FileName;
                return true;
            }
            catch (Exception e)
            {
                applicationPath = null;
                return false;
            }
        }
    }
}
