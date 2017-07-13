using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog
{
    internal static class Win32
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr handle);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr handle, StringBuilder text, int maxCount);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr handle);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr handle, out uint processId);
        
        public static string GetWindowText(IntPtr handle)
        {
            int count = Win32.GetWindowTextLength(handle) + 1;
            StringBuilder builder = new StringBuilder(count);
            Win32.GetWindowText(handle, builder, count);

            return builder.ToString();
        }

        [DllImport("psapi.dll")]
        public static extern uint GetProcessImageFileName(IntPtr handle, StringBuilder fileName, int size);
    }
}
