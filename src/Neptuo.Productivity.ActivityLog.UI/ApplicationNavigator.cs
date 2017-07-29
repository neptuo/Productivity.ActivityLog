using Neptuo;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Formatters;
using Neptuo.Productivity.ActivityLog.Events;
using Neptuo.Productivity.ActivityLog.Properties;
using Neptuo.Productivity.ActivityLog.Services;
using Neptuo.Productivity.ActivityLog.Services.Models;
using Neptuo.Productivity.ActivityLog.ViewModels;
using Neptuo.Productivity.ActivityLog.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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
        private readonly WindowContextFactory contextFactory;

        private WindowContext<object, TodayOverview, OverviewViewModel> todayOverview;
        private WindowContext<bool, Configuration, ConfigurationViewModel> configuration;
        private WindowContext<ICategory, CategoryEdit, CategoryEditViewModel> categoryEdit;

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
            this.contextFactory = new WindowContextFactory(eventHandlers, synchronizer);
        }

        public Task TodayOverview()
        {
            if (todayOverview == null || todayOverview.IsDisposed)
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

                TodayOverview window = new TodayOverview(viewModel, this);
                todayOverview = contextFactory.Create<object, TodayOverview, OverviewViewModel>(window);

                todayOverview.UiThreadHandlers.Add(eventHandlers.AddUiThread<ActivityStarted>(viewModel, synchronizer));
                todayOverview.UiThreadHandlers.Add(eventHandlers.AddUiThread<ActivityEnded>(viewModel, synchronizer));
            }

            todayOverview.Window.Show();
            todayOverview.Window.Activate();

            return todayOverview.CompletionSource.Task;
        }
        
        public Task<bool> Configuration()
        {
            if (configuration == null || configuration.IsDisposed)
            {
                TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>();

                ConfigurationViewModel viewModel = new ConfigurationViewModel(
                    this, 
                    Settings.Default, 
                    new TaskCompletionSourceNavigationHandler<bool>(completionSource)
                );
                viewModel.Categories.Items.AddRange(Settings.Default.Categories.Select(c =>
                {
                    CategoryViewModel category = new CategoryViewModel()
                    {
                        Name = c.Name,
                        Color = c.Color
                    };
                    category.Rules.AddRange(c.Rules.Select(r => new RuleViewModel()
                    {
                        ApplicationPath = r.ApplicationPath,
                        WindowTitle = r.WindowTitle
                    }));
                    return category;
                }));

                Configuration window = new Configuration(viewModel);
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                configuration = contextFactory.Create<bool, Configuration, ConfigurationViewModel>(window, completionSource);
            }

            configuration.Window.Show();
            configuration.Window.Activate();

            return configuration.CompletionSource.Task;
        }

        private Task<ICategory> NewOrEditCategory(ICategory category)
        {
            if (categoryEdit == null || categoryEdit.IsDisposed)
            {
                TaskCompletionSource<ICategory> completionSource = new TaskCompletionSource<ICategory>();
                CategoryEditViewModel viewModel = new CategoryEditViewModel(new TaskCompletionSourceNavigationHandler<ICategory>(completionSource));
                if (category == null)
                {
                    viewModel.Color = Colors.Red;
                }
                else
                {
                    viewModel.Color = category.Color;
                    viewModel.Name = category.Name;
                    viewModel.Rules.AddRange(category.Rules.Select(r => new RuleViewModel()
                    {
                        ApplicationPath = r.ApplicationPath,
                        WindowTitle = r.WindowTitle
                    }));
                }

                CategoryEdit window = new CategoryEdit(viewModel);


                if (configuration != null && !configuration.IsDisposed)
                {
                    window.Owner = configuration.Window;
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }

                categoryEdit = contextFactory.Create<ICategory, CategoryEdit, CategoryEditViewModel>(window, completionSource);
            }

            categoryEdit.Window.Show();
            categoryEdit.Window.Activate();
            return categoryEdit.CompletionSource.Task;
        }

        public Task<ICategory> NewCategory()
        {
            return NewOrEditCategory(null);
        }

        public Task<ICategory> EditCategory(ICategory category)
        {
            Ensure.NotNull(category, "category");
            return NewOrEditCategory(category);
        }

        public Task Message(string message)
        {
            MessageBox.Show(message, "ActivityLog");
            return Task.CompletedTask;
        }

        public Task Message(string title, string message)
        {
            MessageBox.Show(message, "ActivityLog :: " + title);
            return Task.CompletedTask;
        }

        public Task<bool> Confirm(string message)
        {
            return Task.FromResult(MessageBox.Show(message, "ActivityLog", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }

        public Task<bool> Confirm(string title, string message)
        {
            return Task.FromResult(MessageBox.Show(message, "ActivityLog :: " + title, MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }

        public void Exit()
        {
            application.Shutdown();
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            todayOverview?.Dispose();
            configuration?.Dispose();
            categoryEdit?.Dispose();
        }
        
        private class WindowContextFactory
        {
            private readonly IEventHandlerCollection eventHandlers;
            private readonly ISynchronizer synchronizer;

            public WindowContextFactory(IEventHandlerCollection eventHandlers, ISynchronizer synchronizer)
            {
                Ensure.NotNull(eventHandlers, "eventHandlers");
                Ensure.NotNull(synchronizer, "synchronizer");
                this.eventHandlers = eventHandlers;
                this.synchronizer = synchronizer;
            }

            public WindowContext<TResult, TWindow, TViewModel> Create<TResult, TWindow, TViewModel>(TWindow window)
                where TWindow : Window, IViewModel<TViewModel>
            {
                return new WindowContext<TResult, TWindow, TViewModel>(window, eventHandlers, synchronizer);
            }

            public WindowContext<TResult, TWindow, TViewModel> Create<TResult, TWindow, TViewModel>(TWindow window, TaskCompletionSource<TResult> completionSource)
                where TWindow : Window, IViewModel<TViewModel>
            {
                return new WindowContext<TResult, TWindow, TViewModel>(window, eventHandlers, synchronizer, completionSource);
            }
        }

        private class WindowContext<TResult, TWindow, TViewModel> : DisposableBase
            where TWindow : Window, IViewModel<TViewModel>
        {
            private IEventHandlerCollection eventHandlers;
            private ISynchronizer synchronizer;

            public TWindow Window { get; private set; }
            public TaskCompletionSource<TResult> CompletionSource { get; set; }
            public List<UiThreadEventHandler> UiThreadHandlers { get; private set; }

            public WindowContext(TWindow window, IEventHandlerCollection eventHandlers, ISynchronizer synchronizer)
                : this(window, eventHandlers, synchronizer, new TaskCompletionSource<TResult>())
            { }

            public WindowContext(TWindow window, IEventHandlerCollection eventHandlers, ISynchronizer synchronizer, TaskCompletionSource<TResult> completionSource)
            {
                this.eventHandlers = eventHandlers;
                this.synchronizer = synchronizer;

                CompletionSource = completionSource;
                CompletionSource.Task.ContinueWith(OnTaskCompleted);
                Window = window;
                Window.Closed += OnWindowClosed;
                UiThreadHandlers = new List<UiThreadEventHandler>();
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
                Dispose();
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
