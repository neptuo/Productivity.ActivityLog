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
        private int order;
        private ImageSource icon;
        private string processFileName;
        private TimeSpan usedTime;
        private IntPtr windowHandle;
        private ObservableCollection<ActivityRunModel> previousRuns;
        private bool isHidden;

        internal Func<long> GetAllTicks;

        public int Order
        {
            get { return order; }
            set
            {
                if (order != value)
                {
                    order = value;
                    OnPropertyChanged(m => m.Order);
                }
            }
        }

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
                OnPropertyChanged(m => m.Percentage);
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

        [XmlIgnore]
        public string Percentage
        {
            get { return String.Format("{0}%", GetAllTicks != null ? (long)(((double)UsedTime.Ticks / GetAllTicks()) * 100) : 0); }
        }

        public bool IsHidden
        {
            get { return isHidden; }
            set
            {
                if (isHidden != value)
                {
                    isHidden = value;
                    OnPropertyChanged(m => m.IsHidden);
                }
            }
        }

        public ActivityModel()
        {
            PreviousRuns = new ObservableCollection<ActivityRunModel>();
        }

        public void OnPercentageChanged()
        {
            OnPropertyChanged(m => m.Percentage);
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
