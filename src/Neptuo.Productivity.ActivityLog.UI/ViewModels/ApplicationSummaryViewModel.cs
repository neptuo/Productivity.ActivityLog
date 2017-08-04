using Neptuo.Events.Handlers;
using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.ActivityLog.Events;
using Neptuo.Productivity.ActivityLog.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.ViewModels
{
    public class ApplicationSummaryViewModel : ObservableObject, IEventHandler<ActivityStarted>, IEventHandler<ActivityEnded>
    {
        private readonly IApplicationNameProvider applicationNameProvider;
        private readonly IHistoryApplier historyApplier;

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
                    Reload();
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
                    Reload();
                }
            }
        }

        public ObservableCollection<ApplicationDurationViewModel> Applications { get; private set; }

        public ApplicationSummaryViewModel(IApplicationNameProvider applicationNameProvider, IHistoryApplier historyApplier)
        {
            Ensure.NotNull(applicationNameProvider, "applicationNameProvider");
            Ensure.NotNull(historyApplier, "historyApplier");
            this.applicationNameProvider = applicationNameProvider;
            this.historyApplier = historyApplier;

            Applications = new ObservableCollection<ApplicationDurationViewModel>();
        }

        private void Reload()
        {
            if (DateFrom != null && DateTo != null && DateFrom.Value <= DateTo.Value)
            {
                Applications.Clear();
                historyApplier.Apply(this, DateFrom.Value, DateTo.Value);
            }
        }

        Task IEventHandler<ActivityStarted>.HandleAsync(ActivityStarted payload)
        {
            GetViewModel(payload.ApplicationPath).StartAt(payload.WindowTitle, payload.StartedAt);
            return Task.CompletedTask;
        }

        Task IEventHandler<ActivityEnded>.HandleAsync(ActivityEnded payload)
        {
            GetViewModel(payload.ApplicationPath).StopAt(payload.WindowTitle, payload.EndedAt);
            return Task.CompletedTask;
        }

        private ApplicationDurationViewModel GetViewModel(string applicationPath)
        {
            ApplicationDurationViewModel viewModel = Applications.FirstOrDefault(a => a.Path == applicationPath);
            if (viewModel == null)
                Applications.Add(viewModel = new ApplicationDurationViewModel(applicationNameProvider.GetName(applicationPath), applicationPath));

            return viewModel;
        }
    }
}
