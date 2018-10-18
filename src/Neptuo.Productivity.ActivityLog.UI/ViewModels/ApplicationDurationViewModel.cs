using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.ActivityLog.Services;
using Neptuo.Productivity.ActivityLog.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Neptuo.Productivity.ActivityLog.ViewModels
{
    public class ApplicationDurationViewModel : ObservableObject
    {
        private readonly Duration durationCalculator = new Duration();

        public ImageSource Icon { get; private set; }
        public string Path { get; private set; }
        public string Name { get; set; }

        public ObservableCollection<WindowTitleDurationViewModel> Windows { get; private set; }

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

        public ApplicationDurationViewModel(string name, string path)
        {
            Ensure.NotNullOrEmpty(name, "name");
            Ensure.NotNullOrEmpty(path, "path");
            Path = path;
            Name = name;
            Icon = IconExtractor.Get(path);
            Windows = new ObservableCollection<WindowTitleDurationViewModel>();
        }

        public void StartAt(string windowTitle, DateTime startedAt)
        {
            durationCalculator.StartAt(startedAt);
            GetViewModel(windowTitle).StartAt(startedAt);
        }

        public void StopAt(string windowTitle, DateTime endedAt)
        {
            durationCalculator.StopAt(endedAt);
            Duration = durationCalculator.TimeSpan;

            GetViewModel(windowTitle).StopAt(endedAt);
        }

        private WindowTitleDurationViewModel GetViewModel(string windowTitle)
        {
            if (string.IsNullOrEmpty(windowTitle))
                windowTitle = "(empty)";

            WindowTitleDurationViewModel viewModel = Windows.FirstOrDefault(w => w.Title == windowTitle);
            if (viewModel == null)
                Windows.Add(viewModel = new WindowTitleDurationViewModel(windowTitle));

            return viewModel;
        }
    }
}
