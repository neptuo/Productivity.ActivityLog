using Neptuo;
using Neptuo.Events.Handlers;
using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.ActivityLog.Events;
using Neptuo.Productivity.ActivityLog.Services;
using Neptuo.Productivity.ActivityLog.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.ViewModels
{
    public class TodayCategoryViewModel : ObservableObject, IEventHandler<ActivityStarted>, IEventHandler<ActivityEnded>, IDisposable
    {
        private readonly ICategoryResolver resolver;
        private readonly ITimer timer;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly ObservableCollection<CategoryOverviewViewModel> activities;

        public IList<CategoryOverviewViewModel> Activities
        {
            get { return activities; }
        }

        public TodayCategoryViewModel(ICategoryResolver resolver, ITimer timer, IDateTimeProvider dateTimeProvider)
        {
            Ensure.NotNull(resolver, "resolver");
            Ensure.NotNull(timer, "timer");
            Ensure.NotNull(dateTimeProvider, "dateTimeProvider");
            this.resolver = resolver;
            this.timer = timer;
            this.dateTimeProvider = dateTimeProvider;

            timer.Tick += OnTimerTick;
            activities = new ObservableCollection<CategoryOverviewViewModel>();
        }

        private void OnTimerTick()
        {
            DateTime now = dateTimeProvider.Now();
            foreach (CategoryOverviewViewModel item in activities)
            {
                if (item.IsForeground)
                    item.Update(dateTimeProvider.Now());
            }
        }

        Task IEventHandler<ActivityStarted>.HandleAsync(ActivityStarted payload)
        {
            ICategory category = resolver.TryResolve(payload.ApplicationPath, payload.WindowTitle);
            if (category != null)
            {
                CategoryOverviewViewModel viewModel = activities.FirstOrDefault(vm => vm.Name == category.Name);
                if (viewModel != null)
                    viewModel.StartAt(payload.StartedAt);
            }

            return Task.CompletedTask;
        }

        Task IEventHandler<ActivityEnded>.HandleAsync(ActivityEnded payload)
        {
            ICategory category = resolver.TryResolve(payload.ApplicationPath, payload.WindowTitle);
            if (category != null)
            {
                CategoryOverviewViewModel viewModel = activities.FirstOrDefault(vm => vm.Name == category.Name);
                if (viewModel != null)
                    viewModel.StopAt(payload.EndedAt);
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer.Tick -= OnTimerTick;
        }
    }
}
