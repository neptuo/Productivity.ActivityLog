using Neptuo.Events;
using Neptuo.Productivity.ActivityLog.Services;
using Neptuo.Productivity.ActivityLog.UI.ViewModels;
using Neptuo.Windows.Threading;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace Neptuo.Productivity.ActivityLog.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ITimer, ISynchronizer
    {
        private DispatcherHelper dispatcher;
        private Timer timer;
        private DomainService service;

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
            dispatcher = new DispatcherHelper(Dispatcher);
            timer = new Timer(1000);
            timer.Elapsed += OnTimerElapsed;
            timer.Start();

            OverviewViewModel viewModel = new OverviewViewModel(this, this);

            DefaultEventManager eventManager = new DefaultEventManager();
            eventManager.AddAll(viewModel);

            service = new DomainService(eventManager);

            MainWindow wnd = new MainWindow();
            wnd.DataContext = viewModel;
            wnd.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            timer.Dispose();
            service.Dispose();
        }
    }
}
