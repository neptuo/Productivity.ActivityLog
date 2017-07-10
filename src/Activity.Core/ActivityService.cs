using Activity.Core.Models;
using Neptuo.DesktopCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;

namespace Activity.Core
{
    public class ActivityService
    {
        private HashSet<string> currentActivities = new HashSet<string>();
        private HashSet<string> allActivities = new HashSet<string>();
        private ActivityModel currentActivity = null;

        public ObservableCollection<ActivityModel> CurrentActivities { get; private set; }
        public ObservableCollection<ActivityModel> Activities { get; private set; }

        public event EventHandler<CurrentActivityEventArgs> CurrentActivityChanged;

        public ActivityService()
        {
            CurrentActivities = new ObservableCollection<ActivityModel>();
            Activities = new ObservableCollection<ActivityModel>();
        }

        public void ProcessActivity(ActiveWindowChangedEventArgs args)
        {
            try
            {
                bool added = false;
                string fileName = args.CurrentProcess.MainModule.FileName;
                ActivityModel model = null;
                if (!currentActivities.Add(fileName))
                {
                    model = CurrentActivities.FirstOrDefault(m => m.ProcessFileName == fileName);
                    if (model != null)
                    {
                        model.UsedTime = new TimeSpan(model.UsedTime.Add(TimeSpan.FromSeconds(1)).Ticks);
                        model.WindowHandle = args.WindowHandle;
                        added = true;
                    }
                }
                else if (allActivities.Contains(fileName))
                {
                    model = Activities.FirstOrDefault(m => m.ProcessFileName == fileName);
                    if (model != null)
                    {
                        ActivityRunModel runModel = model.PreviousRuns.FirstOrDefault(m => m.Date.Date == DateTime.Now.Date);
                        if (runModel != null)
                            model.UsedTime = new TimeSpan(runModel.Duration.Ticks);

                        model.UsedTime = new TimeSpan(model.UsedTime.Add(TimeSpan.FromSeconds(1)).Ticks);
                        model.WindowHandle = args.WindowHandle;
                        CurrentActivities.Add(model);
                        added = true;
                    }
                }

                if (!added)
                {
                    model = new ActivityModel
                    {
                        Order = CurrentActivities.Count + 1,
                        Icon = IconHelper.GetIcon(fileName),
                        ProcessFileName = fileName,
                        UsedTime = new TimeSpan(),
                        WindowHandle = args.WindowHandle,
                        GetAllTicks = GetAllTicks
                    };
                    Activities.Add(model);
                    CurrentActivities.Add(model);
                }

                if (currentActivity == null || currentActivity.ProcessFileName != fileName)
                {
                    currentActivity = model;
                    if (CurrentActivityChanged != null)
                        CurrentActivityChanged(this, new CurrentActivityEventArgs(currentActivity));
                }

                if (model != null)
                {
                    foreach (ActivityModel item in Activities)
                        item.OnPercentageChanged();
                }
            }
            catch (Exception e) 
            { 
#if DEBUG
                throw e;
#endif
            }
        }

        public ActivityModel GetActivity(Process process)
        {
            return GetActivity(process.MainModule.FileName);
        }

        public ActivityModel GetActivity(string processFileName)
        {
            foreach (ActivityModel model in Activities)
            {
                if (model.ProcessFileName == processFileName)
                    return model;
            }
            return null;
        }

        public void Save(string fileName)
        {
            foreach (ActivityModel model in CurrentActivities)
            {
                if (model.UsedTime.Ticks > 0)
                {
                    ActivityRunModel runModel = model.PreviousRuns.FirstOrDefault(m => m.Date.Date == DateTime.Now.Date);
                    if (runModel != null)
                    {
                        runModel.Duration = new TimeSpan(model.UsedTime.Ticks);
                    }
                    else
                    {
                        model.PreviousRuns.Add(new ActivityRunModel
                        {
                            Date = DateTime.Now.Date,
                            Duration = new TimeSpan(model.UsedTime.Ticks)
                        });
                    }
                }
            }

            using (StreamWriter writer = new StreamWriter(fileName))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<ActivityModel>));
                    serializer.Serialize(writer, Activities);
                }
                catch (Exception) { }
            }
        }

        public void Load(string fileName)
        {
            if (!File.Exists(fileName))
                return;

            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<ActivityModel>));
            using (StreamReader reader = new StreamReader(fileName))
            {
                try
                {
                    Activities = (ObservableCollection<ActivityModel>)serializer.Deserialize(reader);
                    int order = 0;
                    foreach (ActivityModel model in Activities)
                    {
                        model.Order = ++order;
                        model.Icon = IconHelper.GetIcon(model.ProcessFileName);
                        model.GetAllTicks = GetAllTicks;
                        allActivities.Add(model.ProcessFileName);
                    }
                }
                catch (Exception) { }
            }
        }

        private long GetAllTicks()
        {
            long result = 0;
            foreach (ActivityModel model in Activities)
                result += model.UsedTime.Ticks;

            return result;
        }
    }

    public class CurrentActivityEventArgs : EventArgs
    {
        public ActivityModel CurrentActivity { get; private set; }

        public CurrentActivityEventArgs(ActivityModel currentActivity)
        {
            CurrentActivity = currentActivity;
        }
    }
}
