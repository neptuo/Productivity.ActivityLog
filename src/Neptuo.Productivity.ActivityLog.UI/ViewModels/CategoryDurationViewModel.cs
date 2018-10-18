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
        private readonly Duration durationCalculator = new Duration();

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

        public void Reset()
        {
            Duration = TimeSpan.Zero;
        }
    }
}
