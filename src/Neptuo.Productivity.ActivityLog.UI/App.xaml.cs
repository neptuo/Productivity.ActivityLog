using Neptuo.Diagnostics;
using Neptuo.Events;
using Neptuo.Formatters;
using Neptuo.Productivity.ActivityLog.Data;
using Neptuo.Productivity.ActivityLog.Formatters;
using Neptuo.Productivity.ActivityLog.Properties;
using Neptuo.Productivity.ActivityLog.Services;
using Neptuo.Productivity.ActivityLog.Views.Controls;
using Neptuo.Windows.Threading;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Application = System.Windows.Application;
using NotifyIcon = System.Windows.Forms.NotifyIcon;
using ContextMenu = System.Windows.Forms.ContextMenu;
using MessageBox = System.Windows.MessageBox;
using System.Windows.Threading;
using Neptuo.Exceptions.Handlers;
using Neptuo.Logging.Serialization.Formatters;
using Neptuo.Logging;
using Neptuo.Productivity.ActivityLog.Services.Exceptions;
using Neptuo.Formatters.Converters;
using Neptuo.Converters;

namespace Neptuo.Productivity.ActivityLog
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISynchronizer
    {
        private DispatcherHelper dispatcher;
        private DomainService service;
        private DefaultEventManager eventManager;
        private RecoveryService recovery;
        private UiThreadTimer timer;
        private SimpleFormatter formatter;
        private ApplicationNavigator navigator;
        private ApplicationTrayIcon trayIcon;
        private IExceptionHandler exceptionHandler;
        private ILog log;

        public event Action Tick;

        public void Run(Action handler)
        {
            dispatcher.Run(handler);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            dispatcher = new DispatcherHelper(Dispatcher);
            timer = new UiThreadTimer(this);

            eventManager = new DefaultEventManager();

            formatter = new SimpleFormatter();
            navigator = new ApplicationNavigator(this, timer, this, eventManager, formatter, GetEventStoreFileName);
            trayIcon = new ApplicationTrayIcon(navigator);

            SettingsExtension.Settings = Settings.Default;

            BootstrapAsync();
        }

        private async Task BootstrapAsync()
        {
            Converts.Repository
                .AddJsonPrimitivesSearchHandler()
                .AddJsonObjectSearchHandler()
                .AddJsonEnumSearchHandler()
                .AddEnumSearchHandler(false)
                .AddToStringSearchHandler();

            BootstrapErrorHandler();

            FileEventStore store = new FileEventStore(formatter, GetEventStoreFileName);
            eventManager.AddAll(new EventStoreHandler(store));

            await RecoverAsync(formatter);

            service = new DomainService(eventManager);
        }

        private void BootstrapErrorHandler()
        {
            ILogFactory logFactory = new DefaultLogFactory()
                //.AddSerializer(new ErrorLog(new DefaultLogFormatter()))
#if DEBUG
                .AddConsole()
#endif
            ;

            log = logFactory.Scope("Root");

            ExceptionHandlerBuilder builder = new ExceptionHandlerBuilder();
            builder
                .Handler(new LogExceptionHandler(log))
                .Handler(new MessageExceptionHandler(navigator));

            exceptionHandler = builder;
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
            timer.Dispose();
            service.Dispose();

            if (trayIcon != null)
                trayIcon.Dispose();

            base.OnExit(e);
        }

        #region Handling exceptions

        public static void ShowExceptionDialog(Exception e)
        {
            ((App)Current).ShowExceptionDialogInternal(e);
        }

        private void ShowExceptionDialogInternal(Exception e)
        {
            AggregateException aggregate = e as AggregateException;
            if (aggregate != null)
                e = aggregate.InnerException;

            exceptionHandler.Handle(e);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ShowExceptionDialog(e.Exception);
            e.Handled = true;
        }

        private void OnTaskSchedulerUnobservedException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            ShowExceptionDialog(e.Exception);
            e.SetObserved();
        }

        #endregion
    }
}
