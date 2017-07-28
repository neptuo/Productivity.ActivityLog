using Neptuo;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Formatters;
using Neptuo.Productivity.ActivityLog.Events;
using Neptuo.Productivity.ActivityLog.Services;
using Neptuo.Productivity.ActivityLog.ViewModels;
using Neptuo.Productivity.ActivityLog.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Neptuo.Productivity.ActivityLog
{
    internal class ApplicationNavigator : DisposableBase, INavigator
    {
        private readonly App application;
        private readonly ITimer timer;
        private readonly ISynchronizer synchronizer;
        private readonly IEventHandlerCollection eventHandlers;
        private readonly IFormatter formatter;
        private readonly Func<DateTime, string> eventStoreFileNameGetter;

        private TodayOverview todayOverview;
        private UiThreadEventHandler<ActivityStarted> todayOverviewActivityStartedHandler;
        private UiThreadEventHandler<ActivityEnded> todayOverviewActivityEndedHandler;

        private Configuration configuration;

        public ApplicationNavigator(App application, ITimer timer, ISynchronizer synchronizer, IEventHandlerCollection eventHandlers, IFormatter formatter, Func<DateTime, string> eventStoreFileNameGetter)
        {
            Ensure.NotNull(application, "application");
            Ensure.NotNull(timer, "timer");
            Ensure.NotNull(synchronizer, "synchronizer");
            Ensure.NotNull(eventHandlers, "eventHandlers");
            Ensure.NotNull(formatter, "formatter");
            Ensure.NotNull(eventStoreFileNameGetter, "eventStoreFileNameGetter");
            this.application = application;
            this.timer = timer;
            this.synchronizer = synchronizer;
            this.eventHandlers = eventHandlers;
            this.formatter = formatter;
            this.eventStoreFileNameGetter = eventStoreFileNameGetter;
        }

        public void TodayOverview()
        {
            if (todayOverview == null)
            {
                OverviewViewModel viewModel = new OverviewViewModel(
                    timer,
                    new DateTimeProvider(),
                    new ApplicationNameProvider()
                );

                string todayFile = eventStoreFileNameGetter(DateTime.Today);
                if (File.Exists(todayFile))
                {
                    using (Stream file = File.OpenRead(todayFile))
                    {
                        IDeserializerContext context = new DefaultDeserializerContext(typeof(IEnumerable<IEvent>));
                        if (formatter.TryDeserialize(file, context))
                        {
                            foreach (IEvent output in (IEnumerable<IEvent>)context.Output)
                            {
                                if (output is ActivityStarted started)
                                    ((IEventHandler<ActivityStarted>)viewModel).HandleAsync(started).Wait();
                                else if (output is ActivityEnded ended)
                                    ((IEventHandler<ActivityEnded>)viewModel).HandleAsync(ended).Wait();
                            }
                        }
                    }
                }

                todayOverviewActivityStartedHandler = eventHandlers.AddUiThread<ActivityStarted>(viewModel, synchronizer);
                todayOverviewActivityEndedHandler = eventHandlers.AddUiThread<ActivityEnded>(viewModel, synchronizer);

                todayOverview = new TodayOverview(viewModel);
                todayOverview.Closed += OnTodayOverviewClosed;
            }

            todayOverview.Show();
            todayOverview.Activate();
        }

        private void OnTodayOverviewClosed(object sender, EventArgs e)
        {
            TryDisposeTodayOverview();
        }

        public void Configuration()
        {
            if (configuration == null)
            {
                //ConfigurationViewModel viewModel = new ConfigurationViewModel();
                ConfigurationViewModel viewModel = Views.DesignData.ViewModelLocator.Configuration;
                configuration = new Configuration(viewModel);
                configuration.Closed += OnConfigurationClosed;
            }

            configuration.Show();
            configuration.Activate();
        }

        private void OnConfigurationClosed(object sender, EventArgs e)
        {
            TryDisposeConfiguration();
        }

        public void Message(string message)
        {
            MessageBox.Show(message, "ActivityLog");
        }

        public void Message(string title, string message)
        {
            MessageBox.Show(message, "ActivityLog :: " + title);
        }

        public bool Confirm(string message)
        {
            return MessageBox.Show(message, "ActivityLog", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }

        public bool Confirm(string title, string message)
        {
            return MessageBox.Show(message, "ActivityLog :: " + title, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }

        public void Exist()
        {
            application.Shutdown();
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            TryDisposeTodayOverview();
            TryDisposeConfiguration();
        }

        private void TryDisposeTodayOverview()
        {
            if (todayOverview?.ViewModel != null)
            {
                eventHandlers
                    .Remove(todayOverviewActivityStartedHandler)
                    .Remove(todayOverviewActivityEndedHandler);

                todayOverview.Closed -= OnTodayOverviewClosed;
                todayOverview = null;
            }
        }

        private void TryDisposeConfiguration()
        {
            if (configuration?.ViewModel != null)
            {
                configuration.Closed -= OnConfigurationClosed;
                configuration = null;
            }
        }
    }
}
