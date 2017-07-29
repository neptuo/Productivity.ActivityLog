using Neptuo.Events;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.ActivityLog.Events;
using Neptuo.Productivity.ActivityLog.Properties;
using Neptuo.Productivity.ActivityLog.Services;
using Neptuo.Productivity.ActivityLog.Services.Models;
using Neptuo.Productivity.ActivityLog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Neptuo.Productivity.ActivityLog.Views.DesignData
{
    internal class ViewModelLocator
    {
        private static TodayOverviewViewModel overview;

        public static TodayOverviewViewModel Overview
        {
            get
            {
                if (overview == null)
                {
                    overview = new TodayOverviewViewModel(
                        new Timer(),
                        new DateTimeProvider(),
                        new ApplicationNameProvider()
                    );
                    EventManager.AddAll(overview);
                    GenerateEventStream();
                }

                return overview;
            }
        }

        private static ConfigurationViewModel configuration;

        public static ConfigurationViewModel Configuration
        {
            get
            {
                if (configuration == null)
                {
                    configuration = new ConfigurationViewModel(new Navigator(), Settings.Default, new NavigationHandler<bool>());
                    configuration.Categories.Items.Add(new CategoryViewModel
                    {
                        Color = Colors.Red,
                        Name = "Work"
                    });
                    configuration.Categories.Items.Add(new CategoryViewModel
                    {
                        Color = Colors.Green,
                        Name = "Fun"
                    });
                    configuration.Categories.Items.Add(new CategoryViewModel
                    {
                        Color = Colors.Brown,
                        Name = "School"
                    });
                }

                return configuration;
            }
        }

        private static CategoryEditViewModel categoryEdit;

        public static CategoryEditViewModel CategoryEdit
        {
            get
            {
                if (categoryEdit == null)
                {
                    categoryEdit = new CategoryEditViewModel(new NavigationHandler<ICategory>())
                    {
                        Color = Colors.Red,
                        Name = "Work"
                    };
                    categoryEdit.Rules.Add(new RuleViewModel()
                    {
                        ApplicationPath = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe",
                        WindowTitle = "*"
                    });
                    categoryEdit.Rules.Add(new RuleViewModel()
                    {
                        ApplicationPath = @"C:\Program Files (x86)\GitExtensions\GitExtensions.exe",
                        WindowTitle = "*Neptuo*"
                    });
                    categoryEdit.Rules.Add(new RuleViewModel()
                    {
                        ApplicationPath = @"C:\Windows\Notepad.exe",
                        WindowTitle = "*"
                    });
                }

                return categoryEdit;
            }
        }

        private static TodayCategoryViewModel todayCategory;

        public static TodayCategoryViewModel TodayCategory
        {
            get
            {
                if (todayCategory == null)
                {
                    CategoryResolver resolver = new CategoryResolver();
                    resolver.ApplicationPathToCategory[@"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe"] = resolver.ApplicationPathToCategory[@"C:\Windows\Notepad.exe"] = new CategoryViewModel()
                    {
                        Name = "Work",
                        Color = Colors.Red
                    };
                    resolver.ApplicationPathToCategory[@"C:\Windows\Explorer.exe"] = new CategoryViewModel()
                    {
                        Name = "Fun",
                        Color = Colors.Blue
                    };

                    todayCategory = new TodayCategoryViewModel(
                        resolver, 
                        new Timer(), 
                        new DateTimeProvider()
                    );
                    EventManager.AddAll(todayCategory);
                    GenerateEventStream();
                }

                return todayCategory;
            }
        }

        #region Services

        public static DefaultEventManager EventManager { get; } = new DefaultEventManager();

        private static void GenerateEventStream()
        {
            DateTime current = DateTime.Now;

            DateTime AddSeconds(int value = 0)
            {
                current = current.AddSeconds(value);
                return current;
            }

            DateTime AddMinutes(int value = 0)
            {
                current = current.AddMinutes(value);
                return current;
            }

            void ActivityStarted(string path, string title, DateTime? when = null)
                => EventManager.PublishAsync(new ActivityStarted(path, title, current)).Wait();

            void ActivityEnded(string path, string title, DateTime when)
                => EventManager.PublishAsync(new ActivityEnded(path, title, when)).Wait();

            void Activity(string path, string title, DateTime ended)
            {
                ActivityStarted(path, title);
                ActivityEnded(path, title, ended);
            }

            // Activities...

            Activity(@"C:\Windows\Notepad.exe", "Notepad", AddSeconds(20));
            Activity(@"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe", "Microsoft Visual Studio", AddMinutes(20));
            Activity(@"C:\Windows\Explorer.exe", "This PC", AddSeconds(15));
            ActivityStarted(@"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe", "Microsoft Visual Studio");
        }

        #endregion
    }
}
