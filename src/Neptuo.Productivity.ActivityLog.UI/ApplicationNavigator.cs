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

        private TaskCompletionSource<object> todayOverviewCompletionSource;

        public Task TodayOverview()
        {
            if (todayOverviewCompletionSource == null)
            {
                todayOverviewCompletionSource = new TaskCompletionSource<object>();

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

            return todayOverviewCompletionSource.Task;
        }

        private void OnTodayOverviewClosed(object sender, EventArgs e)
        {
            TryDisposeTodayOverview();
        }

        private TaskCompletionSource<bool> configurationCompletionSource;

        public Task<bool> Configuration()
        {
            if (configurationCompletionSource == null)
            {
                configurationCompletionSource = new TaskCompletionSource<bool>();
                configurationCompletionSource.Task.ContinueWith(OnConfigurationCompleted);

                ConfigurationViewModel viewModel = new ConfigurationViewModel(this, Settings.Default, new TaskCompletionSourceNavigationHandler<bool>(configurationCompletionSource));
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

                configuration = new Configuration(viewModel);
                configuration.Closed += OnConfigurationClosed;
                configuration.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            configuration.Show();
            configuration.Activate();
            return configurationCompletionSource.Task;
        }

        private void OnConfigurationCompleted(Task<bool> task)
        {
            if (task.IsCompleted && task.Result)
            {
                synchronizer.Run(() =>
                {
                    configurationCompletionSource = null;
                    TryDisposeConfiguration();
                });
            }
        }

        private void OnConfigurationClosed(object sender, EventArgs e)
        {
            TryDisposeConfiguration();
        }

        private TaskCompletionSource<ICategory> categoryEditCompletionSource;
        private CategoryEdit categoryEdit;

        private Task<ICategory> NewOrEditCategory(ICategory category)
        {
            if (categoryEditCompletionSource == null)
            {
                categoryEditCompletionSource = new TaskCompletionSource<ICategory>();

                CategoryEditViewModel viewModel = new CategoryEditViewModel(OnCategoryEditSaved);
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

                categoryEdit = new CategoryEdit(viewModel);
                categoryEdit.Closed += OnCategoryEditClosed;

                if (configuration != null)
                {
                    categoryEdit.Owner = configuration;
                    categoryEdit.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }
            }

            categoryEdit.Show();
            categoryEdit.Activate();
            return categoryEditCompletionSource.Task;
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

        private void OnCategoryEditSaved()
        {
            if (categoryEditCompletionSource != null && categoryEdit != null)
            {
                categoryEditCompletionSource.SetResult(categoryEdit.ViewModel);
                categoryEditCompletionSource = null;
                categoryEdit.Close();
            }
        }

        private void OnCategoryEditClosed(object sender, EventArgs e)
        {
            TryDisposeCategoryEdit();
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

            TryDisposeTodayOverview();
            TryDisposeConfiguration();
        }

        private void TryDisposeTodayOverview()
        {
            if (todayOverviewCompletionSource != null)
            {
                todayOverviewCompletionSource.SetResult(null);
                todayOverviewCompletionSource = null;
            }

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
            if (configurationCompletionSource != null)
            {
                configurationCompletionSource.SetResult(false);
                configurationCompletionSource = null;
            }

            if (configuration?.ViewModel != null)
            {
                configuration.Closed -= OnConfigurationClosed;

                if (configuration.IsVisible)
                    configuration.Close();

                configuration = null;
            }
        }

        private void TryDisposeCategoryEdit()
        {
            if (categoryEditCompletionSource != null)
            {
                categoryEditCompletionSource.SetResult(null);
                categoryEditCompletionSource = null;
            }

            if (categoryEdit?.ViewModel != null)
            {
                categoryEdit.Closed -= OnCategoryEditClosed;
                categoryEdit = null;
            }
        }
    }
}
