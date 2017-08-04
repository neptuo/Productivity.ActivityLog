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
    internal partial class ApplicationNavigator : DisposableBase, INavigator
    {
        private readonly App application;
        private readonly ITimer timer;
        private readonly ISynchronizer synchronizer;
        private readonly IEventHandlerCollection eventHandlers;
        private readonly IHistoryApplier historyApplier;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly WindowContextFactory contextFactory;
        private readonly Settings settings;

        private WindowContext<object, TodayOverview, TodayOverviewViewModel> todayOverview;
        private WindowContext<object, CategorySummary, CategorySummaryViewModel> categorySummary;
        private WindowContext<object, ApplicationSummary, ApplicationSummaryViewModel> applicationSummary;
        private WindowContext<bool, Configuration, ConfigurationViewModel> configuration;
        private WindowContext<ICategory, CategoryEdit, CategoryEditViewModel> categoryEdit;

        public ApplicationNavigator(App application, ITimer timer, ISynchronizer synchronizer, IEventHandlerCollection eventHandlers, IHistoryApplier historyApplier, IDateTimeProvider dateTimeProvider, Settings settings)
        {
            Ensure.NotNull(application, "application");
            Ensure.NotNull(timer, "timer");
            Ensure.NotNull(synchronizer, "synchronizer");
            Ensure.NotNull(eventHandlers, "eventHandlers");
            Ensure.NotNull(historyApplier, "historyApplier");
            Ensure.NotNull(dateTimeProvider, "dateTimeProvider");
            Ensure.NotNull(settings, "settings");
            this.application = application;
            this.timer = timer;
            this.synchronizer = synchronizer;
            this.eventHandlers = eventHandlers;
            this.historyApplier = historyApplier;
            this.dateTimeProvider = dateTimeProvider;
            this.settings = settings;
            this.contextFactory = new WindowContextFactory(eventHandlers, synchronizer, settings);
        }
        
        public Task TodayOverview()
        {
            if (todayOverview == null || todayOverview.IsDisposed)
            {
                TodayOverviewViewModel viewModel = new TodayOverviewViewModel(
                    timer,
                    dateTimeProvider,
                    new ApplicationNameProvider()
                );

                historyApplier.Apply(viewModel, dateTimeProvider.Now());

                TodayOverview window = new TodayOverview(viewModel);
                todayOverview = contextFactory.Create<object, TodayOverview, TodayOverviewViewModel>(window, nameof(Settings.IsMainWindowOpened));

                todayOverview
                    .AddUiThreadHandler<ActivityStarted>(viewModel)
                    .AddUiThreadHandler<ActivityEnded>(viewModel);
            }

            todayOverview.Window.Show();
            todayOverview.Window.Activate();

            return todayOverview.CompletionSource.Task;
        }

        public Task CategorySummary()
        {
            if (categorySummary == null || categorySummary.IsDisposed)
            {
                CategorySummaryViewModel viewModel = new CategorySummaryViewModel(
                    new ApplicationCategoryResolver(Settings.Default),
                    timer,
                    dateTimeProvider,
                    historyApplier
                );

                foreach (ICategory category in Settings.Default.Categories)
                    viewModel.Activities.Add(new CategoryDurationViewModel(category));

                viewModel.DateFrom = viewModel.DateTo = dateTimeProvider.Now();

                CategorySummary window = new CategorySummary(viewModel);
                categorySummary = contextFactory.Create<object, CategorySummary, CategorySummaryViewModel>(window, nameof(Settings.IsCategorySummaryOpened));

                categorySummary
                    .AddUiThreadHandler<ActivityStarted>(viewModel)
                    .AddUiThreadHandler<ActivityEnded>(viewModel);
            }

            categorySummary.Window.Show();
            categorySummary.Window.Activate();

            return categorySummary.CompletionSource.Task;
        }

        public Task ApplicationSummary()
        {
            if(applicationSummary == null || applicationSummary.IsDisposed)
            {
                ApplicationSummaryViewModel viewModel = new ApplicationSummaryViewModel(
                    new ApplicationNameProvider(),
                    historyApplier
                );

                viewModel.DateFrom = viewModel.DateTo = dateTimeProvider.Now();

                ApplicationSummary window = new ApplicationSummary(viewModel);
                applicationSummary = contextFactory.Create<object, ApplicationSummary, ApplicationSummaryViewModel>(window, nameof(Settings.IsApplicationSummaryOpened));
            }

            applicationSummary.Window.Show();
            applicationSummary.Window.Activate();

            return applicationSummary.CompletionSource.Task;
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
                categoryEdit.CompletionSource.Task.ContinueWith(OnCategoryEditCompleted);
            }

            categoryEdit.Window.Show();
            categoryEdit.Window.Activate();
            return categoryEdit.CompletionSource.Task;
        }

        private void OnCategoryEditCompleted(Task<ICategory> task)
        {
            if (task.IsCompleted)
                synchronizer.Run(() => Configuration());
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
            Ensure.NotNullOrEmpty(message, "message");
            MessageBox.Show(message, "ActivityLog");
            return Task.CompletedTask;
        }

        public Task Message(string title, string message)
        {
            Ensure.NotNullOrEmpty(title, "title");
            Ensure.NotNullOrEmpty(message, "message");
            MessageBox.Show(message, "ActivityLog :: " + title);
            return Task.CompletedTask;
        }

        public Task<bool> Confirm(string message)
        {
            Ensure.NotNullOrEmpty(message, "message");
            return Task.FromResult(MessageBox.Show(message, "ActivityLog", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }

        public Task<bool> Confirm(string title, string message)
        {
            Ensure.NotNullOrEmpty(title, "title");
            Ensure.NotNullOrEmpty(message, "message");
            return Task.FromResult(MessageBox.Show(message, "ActivityLog :: " + title, MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }

        public void Exit()
        {
            todayOverview?.DetachSettingsKey();
            categorySummary?.DetachSettingsKey();
            configuration?.DetachSettingsKey();
            categoryEdit?.DetachSettingsKey();

            application.Shutdown();
        }

        internal Task RestorePreviousAsync()
        {
            if (settings.IsMainWindowOpened)
                TodayOverview();

            if (settings.IsCategorySummaryOpened)
                CategorySummary();

            return Task.CompletedTask;
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            settings.Save();

            todayOverview?.Dispose();
            categorySummary?.Dispose();
            configuration?.Dispose();
            categoryEdit?.Dispose();
        }
    }
}
