using Neptuo;
using Neptuo.Events;
using Neptuo.Productivity.ActivityLog.Properties;
using Neptuo.Productivity.ActivityLog.Services;
using Neptuo.Productivity.ActivityLog.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Neptuo.Productivity.ActivityLog
{
    partial class ApplicationNavigator
    {
        private class WindowContextFactory
        {
            private readonly IEventHandlerCollection eventHandlers;
            private readonly ISynchronizer synchronizer;
            private readonly Settings settings;

            public WindowContextFactory(IEventHandlerCollection eventHandlers, ISynchronizer synchronizer, Settings settings)
            {
                Ensure.NotNull(eventHandlers, "eventHandlers");
                Ensure.NotNull(synchronizer, "synchronizer");
                Ensure.NotNull(settings, "settings");
                this.eventHandlers = eventHandlers;
                this.synchronizer = synchronizer;
                this.settings = settings;
            }

            public WindowContext<TResult, TWindow, TViewModel> Create<TResult, TWindow, TViewModel>(TWindow window, string settingsKey = null)
                where TWindow : Window, IView<TViewModel>
            {
                return new WindowContext<TResult, TWindow, TViewModel>(window, eventHandlers, synchronizer, settings, settingsKey);
            }

            public WindowContext<TResult, TWindow, TViewModel> Create<TResult, TWindow, TViewModel>(TWindow window, TaskCompletionSource<TResult> completionSource, string settingsKey = null)
                where TWindow : Window, IView<TViewModel>
            {
                return new WindowContext<TResult, TWindow, TViewModel>(window, eventHandlers, synchronizer, completionSource, settings, settingsKey);
            }
        }
    }
}
