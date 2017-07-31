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
    public class CategorySummaryViewModel : ObservableObject, IEventHandler<ActivityStarted>, IEventHandler<ActivityEnded>, IDisposable
    {
        private readonly ICategoryResolver resolver;
        private readonly ITimer timer;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IHistoryApplier applier;
        private readonly ObservableCollection<CategoryDurationViewModel> activities;

        private DateTime? dateFrom;
        public DateTime? DateFrom
        {
            get { return dateFrom; }
            set
            {
                if (dateFrom != value)
                {
                    if (value == null)
                        dateFrom = value;
                    else
                        dateFrom = value.Value.Date;

                    RaisePropertyChanged();
                    ReloadActivities();
                }
            }
        }

        private DateTime? dateTo;
        public DateTime? DateTo
        {
            get { return dateTo; }
            set
            {
                if (dateTo != value)
                {
                    if (value == null)
                        dateTo = value;
                    else
                        dateTo = value.Value.Date;

                    RaisePropertyChanged();
                    ReloadActivities();
                }
            }
        }

        public IList<CategoryDurationViewModel> Activities
        {
            get { return activities; }
        }

        public CategorySummaryViewModel(ICategoryResolver resolver, ITimer timer, IDateTimeProvider dateTimeProvider, IHistoryApplier applier)
        {
            Ensure.NotNull(resolver, "resolver");
            Ensure.NotNull(timer, "timer");
            Ensure.NotNull(dateTimeProvider, "dateTimeProvider");
            Ensure.NotNull(applier, "applier");
            this.resolver = resolver;
            this.timer = timer;
            this.dateTimeProvider = dateTimeProvider;
            this.applier = applier;

            timer.Tick += OnTimerTick;
            activities = new ObservableCollection<CategoryDurationViewModel>();
        }

        private void OnTimerTick()
        {
            DateTime now = dateTimeProvider.Now();
            foreach (CategoryDurationViewModel item in activities)
            {
                if (item.IsForeground)
                    item.Update(dateTimeProvider.Now());
            }
        }

        public void ReloadActivities()
        {
            if (DateFrom != null && DateTo != null && DateFrom.Value <= DateTo.Value)
            {
                foreach (CategoryDurationViewModel item in activities)
                    item.Reset();

                applier.Apply(this, DateFrom.Value, DateTo.Value);
            }
        }

        Task IEventHandler<ActivityStarted>.HandleAsync(ActivityStarted payload)
        {
            if (IsBetween(payload.StartedAt))
            {
                ICategory category = resolver.TryResolve(payload.ApplicationPath, payload.WindowTitle);
                if (category != null)
                {
                    CategoryDurationViewModel viewModel = activities.FirstOrDefault(vm => vm.Name == category.Name);
                    if (viewModel != null)
                        viewModel.StartAt(payload.StartedAt);
                }
            }

            return Task.CompletedTask;
        }

        Task IEventHandler<ActivityEnded>.HandleAsync(ActivityEnded payload)
        {
            if (IsBetween(payload.EndedAt))
            {
                ICategory category = resolver.TryResolve(payload.ApplicationPath, payload.WindowTitle);
                if (category != null)
                {
                    CategoryDurationViewModel viewModel = activities.FirstOrDefault(vm => vm.Name == category.Name);
                    if (viewModel != null)
                        viewModel.StopAt(payload.EndedAt);
                }
            }

            return Task.CompletedTask;
        }

        private bool IsBetween(DateTime value)
        {
            if (DateFrom == null || DateTo == null)
                return false;

            value = value.Date;
            return DateFrom.Value <= value && DateTo.Value >= value;
        }

        public void Dispose()
        {
            timer.Tick -= OnTimerTick;
        }
    }
}
