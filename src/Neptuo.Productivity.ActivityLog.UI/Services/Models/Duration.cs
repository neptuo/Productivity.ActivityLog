using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.Services.Models
{
    public class Duration
    {
        private TimeSpan lastDuration;
        private DateTime lastStartedAt;
        private DateTime lastEndedAt;

        public TimeSpan TimeSpan { get; set; }

        public void StartAt(DateTime startedAt)
        {
            lastStartedAt = startedAt;
        }

        public void StopAt(DateTime endedAt)
        {
            if (lastStartedAt > DateTime.MinValue)
            {
                lastEndedAt = endedAt;
                lastDuration = lastDuration + (lastEndedAt - lastStartedAt);
                TimeSpan = lastDuration;
            }
        }

        public TimeSpan Update(DateTime now)
        {
            return lastDuration + (now - lastStartedAt);
        }
    }
}
