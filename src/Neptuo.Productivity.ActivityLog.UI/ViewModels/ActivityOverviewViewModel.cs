using Neptuo.Observables;
using Neptuo.Productivity.ActivityLog.Services;
using System;
using System.Collections.Generic;
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

        public ActivityOverviewViewModel(string applicationPath)
        {
            //Ensure.NotNullOrEmpty(applicationPath, "applicationPath");
            Ensure.NotNull(applicationPath, "applicationPath");
            ApplicationPath = applicationPath;

            if (ApplicationPath != String.Empty)
            {
                ApplicationName = Path.GetFileName(ApplicationPath);
                Icon = IconExtractor.Get(ApplicationPath);
            }
        }

        public void StartAt(DateTime startedAt)
        {
            IsForeground = true;
        }

        public void StopAt(DateTime endedAt)
        {
            IsForeground = false;
        }

        public void Update()
        {

        }
    }
}
