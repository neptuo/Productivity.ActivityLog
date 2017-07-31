using Neptuo;
using Neptuo.Observables;
using Neptuo.Productivity.ActivityLog.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Neptuo.Productivity.ActivityLog.ViewModels
{
    public class CategoryDurationViewModel : ObservableObject
    {
        private TimeSpan lastDuration;
        private DateTime lastStartedAt;
        private DateTime lastEndedAt;

        public string Name { get; private set; }
        public Brush ColorBrush { get; private set; }

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
            get { return Duration.TotalSeconds; }
        }

        public CategoryDurationViewModel(ICategory model)
        {
            Ensure.NotNull(model, "category");
            Name = model.Name;
            ColorBrush = new SolidColorBrush(model.Color);
        }

        public void StartAt(DateTime startedAt)
        {
            IsForeground = true;
            lastStartedAt = startedAt;
        }

        public void StopAt(DateTime endedAt)
        {
            if (lastStartedAt > DateTime.MinValue)
            {
                IsForeground = false;
                lastEndedAt = endedAt;
                lastDuration = lastDuration + (lastEndedAt - lastStartedAt);
                Duration = lastDuration;
            }
        }

        public void Update(DateTime now)
        {
            if (IsForeground)
                Duration = lastDuration + (now - lastStartedAt);
        }

        public void Reset()
        {
            Duration = TimeSpan.Zero;
        }
    }
}
