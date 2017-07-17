using Neptuo;
using Neptuo.Events.Handlers;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.ActivityLog.Events;
using Neptuo.Productivity.ActivityLog.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.UI.ViewModels
{
    public class OverviewViewModel : IEventHandler<ActivityStarted>, IEventHandler<ActivityEnded>
    {
        private readonly ITimer timer;
        private readonly ISynchronizer synchronizer;
        private readonly ObservableCollection<ActivityOverviewViewModel> activities;

        public string Title { get; set; } = "Hello!";

        public IReadOnlyList<ActivityOverviewViewModel> Activities
        {
            get { return activities; }
        }

        public OverviewViewModel(ITimer timer, ISynchronizer synchronizer)
        {
            Ensure.NotNull(timer, "timer");
            Ensure.NotNull(synchronizer, "synchronizer");
            this.timer = timer;
            this.synchronizer = synchronizer;
            this.timer.Tick += OnTimerTick;

            activities = new ObservableCollection<ActivityOverviewViewModel>();
        }

        private void OnTimerTick()
        {
            synchronizer.Run(() =>
            {
                foreach (ActivityOverviewViewModel item in activities)
                {
                    if (item.IsForeground)
                        item.Update();
                }
            });
        }

        Task IEventHandler<ActivityStarted>.HandleAsync(ActivityStarted payload)
        {
            synchronizer.Run(() =>
            {
                bool hasItem = false;

                foreach (ActivityOverviewViewModel item in activities)
                {
                    if (payload.ApplicationPath == item.ApplicationPath)
                    {
                        item.CurrentTitle = payload.WindowTitle;
                        item.StartAt(payload.StartedAt);
                        hasItem = true;
                    }
                }

                if (!hasItem)
                {
                    ActivityOverviewViewModel newItem = new ActivityOverviewViewModel(payload.ApplicationPath);
                    newItem.CurrentTitle = payload.WindowTitle;
                    newItem.StartAt(payload.StartedAt);
                    activities.Add(newItem);
                }
            });

            return Task.CompletedTask;
        }

        Task IEventHandler<ActivityEnded>.HandleAsync(ActivityEnded payload)
        {
            synchronizer.Run(() =>
            {
                foreach (ActivityOverviewViewModel item in activities)
                {
                    if (payload.ApplicationPath == item.ApplicationPath)
                        item.StopAt(payload.EndedAt);
                }
            });

            return Task.CompletedTask;
        }
    }
}
