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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using NotifyIcon = System.Windows.Forms.NotifyIcon;
using Application = System.Windows.Application;

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
        private Timer timer;
        private SimpleFormatter formatter;
        private ApplicationNavigator navigator;
        private NotifyIcon trayIcon;

        public event Action Tick;

        public void Run(Action handler)
        {
            dispatcher.Run(handler);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            BootTrayIcon();

            timer = new Timer();

            eventManager = new DefaultEventManager();

            dispatcher = new DispatcherHelper(Dispatcher);
            formatter = new SimpleFormatter();
            navigator = new ApplicationNavigator(timer, this, eventManager, formatter, GetEventStoreFileName);

            BootstrapAsync();
        }

        private void BootTrayIcon()
        {
            if (trayIcon != null)
                return;

            trayIcon = new NotifyIcon();
            trayIcon.Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName);
            trayIcon.Text = "ActivityLog";
            trayIcon.MouseClick += OnTrayIconClick;
            trayIcon.Visible = true;

            trayIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
            trayIcon.ContextMenu.MenuItems.Add("Overview", (sender, e) => { OnTrayIconClick(sender, null); });
            trayIcon.ContextMenu.MenuItems.Add("Exit", (sender, e) => Shutdown());
        }

        private void OnTrayIconClick(object sender, MouseEventArgs e)
        {
            if (e == null || e.Button == MouseButtons.Left)
                navigator.OpenOverview();
        }

        private async Task BootstrapAsync()
        {
            FileEventStore store = new FileEventStore(formatter, GetEventStoreFileName);
            eventManager.AddAll(new EventStoreHandler(store));

            await RecoverAsync(formatter);

            service = new DomainService(eventManager);
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
            {
                trayIcon.Dispose();
                trayIcon = null;
            }

            base.OnExit(e);
        }
    }
}
