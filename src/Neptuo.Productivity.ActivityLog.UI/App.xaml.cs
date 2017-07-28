using Neptuo.Diagnostics;
using Neptuo.Events;
using Neptuo.Formatters;
using Neptuo.Productivity.ActivityLog.Data;
using Neptuo.Productivity.ActivityLog.Events;
using Neptuo.Productivity.ActivityLog.Formatters;
using Neptuo.Productivity.ActivityLog.Services;
using Neptuo.Productivity.ActivityLog.ViewModels;
using Neptuo.Productivity.ActivityLog.Views;
using Neptuo.Windows.Threading;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace Neptuo.Productivity.ActivityLog
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ITimer, ISynchronizer
    {
        private DispatcherHelper dispatcher;
        private Timer timer;
        private DomainService service;
        private DefaultEventManager eventManager;
        private RecoveryService recovery;
        private OverviewViewModel viewModel;

        public event Action Tick;

        public void Run(Action handler)
        {
            dispatcher.Run(handler);
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (Tick != null)
                Tick();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            eventManager = new DefaultEventManager();

            dispatcher = new DispatcherHelper(Dispatcher);
            timer = new Timer(1000);
            timer.Elapsed += OnTimerElapsed;
            timer.Start();

            viewModel = new OverviewViewModel(
                this,
                this,
                new DateTimeProvider(),
                new ApplicationNameProvider()
            );

            eventManager.AddAll(viewModel);

            StartAsync(viewModel);
        }

        private async Task StartAsync(OverviewViewModel viewModel)
        {
            SimpleFormatter formatter = new SimpleFormatter();

            string todayFile = GetEventStoreFileName(DateTime.Today);
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
                                await eventManager.PublishAsync(started);
                            else if (output is ActivityEnded ended)
                                await eventManager.PublishAsync(ended);
                        }
                    }
                }
            }

            FileEventStore store = new FileEventStore(formatter, GetEventStoreFileName);
            eventManager.AddAll(new EventStoreHandler(store));

            await RecoverAsync(formatter);

            service = new DomainService(eventManager);

            await Task.Delay(3000);

            MainWindow wnd = new MainWindow(viewModel);
            wnd.Show();
        }

        private async Task RecoverAsync(IFormatter formatter)
        {
            recovery = new RecoveryService(formatter, () => "recovery.alog");
            await recovery.RecoverAsync(eventManager);
            eventManager.AddAll(recovery);
        }

        private static string GetEventStoreFileName(DateTime dateTime)
        {
            return $"{dateTime.ToString("yyyy-MM-dd")}.alog";
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            timer.Dispose();
            service.Dispose();
        }
    }
}
