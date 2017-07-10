using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Activity.Core
{
    public class ActivityProvider
    {
        private Thread checkingThread = null;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        public event EventHandler<ActiveWindowChangedEventArgs> WindowChanged;

        public void Start()
        {
            checkingThread = new Thread(new ThreadStart(ThreadChecking));
            checkingThread.Start();
        }

        public void Stop()
        {
            checkingThread.Abort();
        }

        public void SetCurrentWindow(IntPtr handle)
        {
            bool result = SetForegroundWindow(handle);
        }

        private void ThreadChecking()
        {
            Process lastProcess = null;
            while (true)
            {
                IntPtr handle = GetForegroundWindow();

                uint processId;
                GetWindowThreadProcessId(handle, out processId);

                try
                {
                    Process currentProcess = Process.GetProcessById((int)processId);
                    if (currentProcess != null)
                    {
                        if (WindowChanged != null)
                            WindowChanged(this, new ActiveWindowChangedEventArgs(currentProcess, lastProcess, handle));

                        lastProcess = currentProcess;
                    }
                }
                catch (ArgumentException) { }

                Thread.Sleep(1000);
            }
        }
    }

    public class ActiveWindowChangedEventArgs : EventArgs
    {
        public Process CurrentProcess { get; private set; }
        public Process LastProcess { get; private set; }
        public IntPtr WindowHandle { get; private set; }

        public ActiveWindowChangedEventArgs(Process currentProcess, Process lastProcess, IntPtr windowHandle)
        {
            CurrentProcess = currentProcess;
            LastProcess = lastProcess;
            WindowHandle = windowHandle;
        }
    }
}
