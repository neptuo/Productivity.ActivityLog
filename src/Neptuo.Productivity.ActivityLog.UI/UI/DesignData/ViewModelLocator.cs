using Neptuo.Events;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.ActivityLog.Events;
using Neptuo.Productivity.ActivityLog.Services;
using Neptuo.Productivity.ActivityLog.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.UI.DesignData
{
    public class ViewModelLocator
    {
        private static OverviewViewModel overview;

        public static OverviewViewModel Overview
        {
            get
            {
                if (overview == null)
                {
                    overview = new OverviewViewModel(new Timer(), new Synchronizer());
                    (overview.Activities as ObservableCollection<ActivityOverviewViewModel>).Add(new ActivityOverviewViewModel("C:/Windows/Notepad.exe"));
                    EventManager.AddAll(overview);

                    //EventManager.PublishAsync(new ActivityStarted("C:/Windows/Notepad.exe", "Notepad", DateTime.Now)).Wait();
                }

                return overview;
            }
        }

        public static DefaultEventManager EventManager { get; } = new DefaultEventManager();

        public class Timer : ITimer
        {
            public event Action Tick;
        }

        public class Synchronizer : ISynchronizer
        {
            public void Run(Action handler)
            {
                handler();
            }
        }
    }
}
