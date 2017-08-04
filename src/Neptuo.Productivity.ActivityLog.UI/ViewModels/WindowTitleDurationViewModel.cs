using Neptuo;
using Neptuo.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.ViewModels
{
    public class WindowTitleDurationViewModel : ObservableObject
    {
        private TimeSpan lastDuration;
        private DateTime lastStartedAt;
        private DateTime lastEndedAt;

        public string Title { get; private set; }

        private TimeSpan duration;
        public TimeSpan Duration
        {
            get { return duration; }
            private set
            {
                if (duration != value)
                {
                    duration = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(DurationRaw));
                }
            }
        }

        public double DurationRaw
        {
            get { return Duration.TotalMilliseconds; }
        }

        public WindowTitleDurationViewModel(string title)
        {
            Title = title;
        }

        public void StartAt(DateTime startedAt)
        {
            lastStartedAt = startedAt;
        }

        public void StopAt(DateTime endedAt)
        {
            lastEndedAt = endedAt;
            lastDuration = lastDuration + (lastEndedAt - lastStartedAt);
            Duration = lastDuration;
        }
    }
}
