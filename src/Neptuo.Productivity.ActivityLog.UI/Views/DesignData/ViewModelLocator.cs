using Neptuo.Events;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.ActivityLog.Events;
using Neptuo.Productivity.ActivityLog.Services;
using Neptuo.Productivity.ActivityLog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.Views.DesignData
{
    internal class ViewModelLocator
    {
        private static OverviewViewModel overview;

        public static OverviewViewModel Overview
        {
            get
            {
                if (overview == null)
                {
                    overview = new OverviewViewModel(new Timer(), new Synchronizer());
                    EventManager.AddAll(overview);
                    GenerateEventStream();
                }

                return overview;
            }
        }

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
            ActivityStarted(@"C:\Windows\Explorer.exe", "This PC");
        }
    }
}
