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
        private readonly ITimer timer;
        private readonly ISynchronizer synchronizer;
        private readonly IEventHandlerCollection eventHandlers;
        private readonly IFormatter formatter;
        private readonly Func<DateTime, string> eventStoreFileNameGetter;

        private MainWindow mainWindow;
        private UiThreadEventHandler<ActivityStarted> mainActivityStartedHandler;
        private UiThreadEventHandler<ActivityEnded> mainActivityEndedHandler;

        public ApplicationNavigator(ITimer timer, ISynchronizer synchronizer, IEventHandlerCollection eventHandlers, IFormatter formatter, Func<DateTime, string> eventStoreFileNameGetter)
        {
            Ensure.NotNull(timer, "timer");
            Ensure.NotNull(synchronizer, "synchronizer");
            Ensure.NotNull(eventHandlers, "eventHandlers");
            Ensure.NotNull(formatter, "formatter");
            Ensure.NotNull(eventStoreFileNameGetter, "eventStoreFileNameGetter");
            this.timer = timer;
            this.synchronizer = synchronizer;
            this.eventHandlers = eventHandlers;
            this.formatter = formatter;
            this.eventStoreFileNameGetter = eventStoreFileNameGetter;
        }

        public void Overview()
        {
            if (mainWindow == null)
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

                mainActivityStartedHandler = eventHandlers.AddUiThread<ActivityStarted>(viewModel, synchronizer);
                mainActivityEndedHandler = eventHandlers.AddUiThread<ActivityEnded>(viewModel, synchronizer);

                mainWindow = new MainWindow(viewModel);
                mainWindow.Closed += OnMainWindowClosed;
            }

            mainWindow.Show();
            mainWindow.Activate();
        }

        private void OnMainWindowClosed(object sender, EventArgs e)
        {
            TryDisposeMainWindow();
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            TryDisposeMainWindow();
        }

        private void TryDisposeMainWindow()
        {
            if (mainWindow?.ViewModel != null)
            {
                eventHandlers
                    .Remove(mainActivityStartedHandler)
                    .Remove(mainActivityEndedHandler);

                mainWindow.Closed -= OnMainWindowClosed;
                mainWindow = null;
            }
        }

        public void Message(string message)
        {
            MessageBox.Show(message, "ActivityLog");
        }

        public void Message(string title, string message)
        {
            MessageBox.Show(message, title);
        }
    }
}
