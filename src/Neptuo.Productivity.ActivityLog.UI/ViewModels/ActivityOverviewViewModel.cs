using Neptuo;
using Neptuo.Observables;
using Neptuo.Productivity.ActivityLog.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Neptuo.Productivity.ActivityLog.ViewModels
{
    public class ActivityOverviewViewModel : ObservableObject
    {
        private TimeSpan lastDuration;
        private DateTime lastStartedAt;
        private DateTime lastEndedAt;

        public ImageSource Icon { get; private set; }
        public string ApplicationPath { get; private set; }
        public string ApplicationName { get; set; }

        private string currentTitle;
        public string CurrentTitle
        {
            get { return currentTitle; }
            set
            {
                if (currentTitle != value)
                {
                    currentTitle = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool isForeground;
        public bool IsForeground
        {
            get { return isForeground; }
            private set
            {
                if (isForeground != value)
                {
                    isForeground = value;
                    RaisePropertyChanged();
                }
            }
        }

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
                }
            }
        }

        public ActivityOverviewViewModel(string applicationName, string applicationPath)
        {
            Ensure.NotNullOrEmpty(applicationName, "applicationName");
            Ensure.NotNullOrEmpty(applicationPath, "applicationPath");
            ApplicationPath = applicationPath;
            ApplicationName = applicationName;
            Icon = IconExtractor.Get(applicationPath);
        }

        public void StartAt(DateTime startedAt)
        {
            IsForeground = true;
            lastStartedAt = startedAt;
        }

        public void StopAt(DateTime endedAt)
        {
            IsForeground = false;
            lastEndedAt = endedAt;
            lastDuration = lastDuration + (lastEndedAt - lastStartedAt);
            Duration = lastDuration;
        }

        public void Update(DateTime now)
        {
            if (IsForeground)
                Duration = lastDuration + (now - lastStartedAt);
        }
    }
}
