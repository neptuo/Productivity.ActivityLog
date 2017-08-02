using Neptuo.Events;
using Neptuo.Events.Handlers;
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
        private class WindowContext<TResult, TWindow, TViewModel> : DisposableBase
            where TWindow : Window, IView<TViewModel>
        {
            private readonly IEventHandlerCollection eventHandlers;
            private readonly ISynchronizer synchronizer;
            private readonly Settings settings;
            private string settingsKey;

            public TWindow Window { get; private set; }
            public TaskCompletionSource<TResult> CompletionSource { get; private set; }
            public List<UiThreadEventHandler> UiThreadHandlers { get; private set; }

            public WindowContext(TWindow window, IEventHandlerCollection eventHandlers, ISynchronizer synchronizer, Settings settings, string settingsKey = null)
                : this(window, eventHandlers, synchronizer, new TaskCompletionSource<TResult>(), settings, settingsKey)
            { }

            public WindowContext(TWindow window, IEventHandlerCollection eventHandlers, ISynchronizer synchronizer, TaskCompletionSource<TResult> completionSource, Settings settings, string settingsKey = null)
            {
                this.eventHandlers = eventHandlers;
                this.synchronizer = synchronizer;
                this.settings = settings;
                this.settingsKey = settingsKey;

                CompletionSource = completionSource;
                CompletionSource.Task.ContinueWith(OnTaskCompleted);
                Window = window;
                Window.Closed += OnWindowClosed;
                UiThreadHandlers = new List<UiThreadEventHandler>();

                if (settingsKey != null)
                    settings[settingsKey] = true;
            }

            public WindowContext<TResult, TWindow, TViewModel> AddUiThreadHandler<TEvent>(IEventHandler<TEvent> handler)
            {
                UiThreadHandlers.Add(eventHandlers.AddUiThread(handler, synchronizer));
                return this;
            }

            private void OnTaskCompleted(Task<TResult> task)
            {
                if (task.IsCompleted && !IsDisposed)
                {
                    synchronizer.Run(() =>
                    {
                        CompletionSource = null;
                        Dispose();
                    });
                }
            }

            private void OnWindowClosed(object sender, EventArgs e)
            {
                if (settingsKey != null)
                    settings[settingsKey] = false;

                Dispose();
            }

            public void DetachSettingsKey()
            {
                settingsKey = null;
            }

            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();

                if (CompletionSource != null)
                {
                    CompletionSource.SetResult(default(TResult));
                    CompletionSource = null;
                }

                if (Window != null)
                {
                    Window.Closed -= OnWindowClosed;

                    if (Window.IsVisible)
                        Window.Close();

                    if (Window.ViewModel != null && Window.ViewModel is IDisposable disposable)
                        disposable.Dispose();

                    Window = null;
                }

                if (UiThreadHandlers.Count > 0)
                {
                    foreach (UiThreadEventHandler handler in UiThreadHandlers)
                        handler.Remove(eventHandlers);

                    UiThreadHandlers.Clear();
                }
            }
        }
    }
}
