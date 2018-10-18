using Neptuo;
using Neptuo.Observables;
using Neptuo.Productivity.ActivityLog.Services;
using Neptuo.Productivity.ActivityLog.Services.Models;
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
        private readonly Duration durationCalculator = new Duration();

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
                    RaisePropertyChanged(nameof(DurationRaw));
                }
            }
        }

        public double DurationRaw
        {
            get { return Duration.TotalMilliseconds; }
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
            durationCalculator.StartAt(startedAt);
        }

        public void StopAt(DateTime endedAt)
        {
            IsForeground = false;
            durationCalculator.StopAt(endedAt);
            Duration = durationCalculator.TimeSpan;
        }

        public void Update(DateTime now)
        {
            if (IsForeground)
                Duration = durationCalculator.Update(now);
        }
    }
}
