using Neptuo.DesktopCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Activity.Core.Models
{
    [XmlRoot("Activity")]
    public class ActivityModel : ObservableObject<ActivityModel>
    {
        private ImageSource icon;
        private string processFileName;
        private TimeSpan usedTime;
        private IntPtr windowHandle;
        private ObservableCollection<ActivityRunModel> previousRuns;

        [XmlIgnore]
        public ImageSource Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                OnPropertyChanged(m => m.Icon);
            }
        }

        [XmlElement("FileName")]
        public string ProcessFileName
        {
            get { return processFileName; }
            set
            {
                processFileName = value;
                OnPropertyChanged(m => m.ProcessFileName);
            }
        }

        [XmlIgnore]
        public TimeSpan UsedTime
        {
            get { return usedTime; }
            set
            {
                usedTime = value;
                OnPropertyChanged(m => m.UsedTime);
            }
        }

        [XmlIgnore]
        public IntPtr WindowHandle
        {
            get { return windowHandle; }
            set
            {
                windowHandle = value;
                OnPropertyChanged(m => m.WindowHandle);
                OnPropertyChanged(m => m.IsCurrent);
            }
        }

        [XmlElement("Runs")]
        public ObservableCollection<ActivityRunModel> PreviousRuns
        {
            get { return previousRuns; }
            set
            {
                if (previousRuns != value)
                {
                    previousRuns = value;
                    OnPropertyChanged(m => m.PreviousRuns);
                }
            }
        }

        [XmlIgnore]
        public bool IsCurrent
        {
            get { return WindowHandle.ToInt32() != 0; }
        }

        public ActivityModel()
        {
            PreviousRuns = new ObservableCollection<ActivityRunModel>();
        }
    }

    public class ActivityRunModel : ObservableObject<ActivityRunModel>
    {
        private DateTime date;
        private TimeSpan duration;

        public DateTime Date
        {
            get { return date; }
            set
            {
                if (date != value)
                {
                    date = value;
                    OnPropertyChanged(m => m.Date);
                }
            }
        }

        [XmlIgnore]
        public TimeSpan Duration
        {
            get { return duration; }
            set
            {
                if (duration != value)
                {
                    duration = value;
                    OnPropertyChanged(m => m.Duration);
                }
            }
        }

        [XmlAttribute("Ticks")]
        public long DurationTicks
        {
            get { return Duration.Ticks; }
            set { Duration = new TimeSpan(value); }
        }
    }
}
