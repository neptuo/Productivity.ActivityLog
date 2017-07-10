using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.Events
{
    /// <summary>
    /// An event raised when a new activity has started.
    /// </summary>
    public class ActivityStarted : Event
    {
        public string ApplicationPath { get; private set; }
        public string WindowTitle { get; private set; }
        public DateTime StartedAt { get; private set; }

        public ActivityStarted(string applicationPath, string windowTitle, DateTime startedAt)
        {
            ApplicationPath = applicationPath;
            WindowTitle = windowTitle;
            StartedAt = startedAt;
        }
    }
}
