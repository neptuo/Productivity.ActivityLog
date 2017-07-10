using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.Events
{
    /// <summary>
    /// An event raised when an activity has ended.
    /// </summary>
    public class ActivityEnded : Event
    {
        public string ApplicationPath { get; private set; }
        public string WindowTitle { get; private set; }
        public DateTime EndedAt { get; private set; }

        public ActivityEnded(string applicationPath, string windowTitle, DateTime endedAt)
        {
            ApplicationPath = applicationPath;
            WindowTitle = windowTitle;
            EndedAt = endedAt;
        }
    }
}
