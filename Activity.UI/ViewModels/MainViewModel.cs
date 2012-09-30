using Activity.Core;
using Activity.Core.Models;
using Neptuo.DesktopCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;

namespace Activity.UI.ViewModels
{
    public class MainViewModel : BaseViewModel<ObservableCollection<ActivityModel>>
    {
        private ActivityModel currentActivity;

        public ActivityModel CurrentActivity
        {
            get { return currentActivity; }
            set
            {
                if (currentActivity != value)
                {
                    currentActivity = value;
                    OnPropertyChanged("CurrentActivity");
                }
            }
        }

        public MainViewModel(ActivityService service)
            : base(service.CurrentActivities)
        {
            Title = "Activity";
            service.CurrentActivityChanged += (sender, e) => CurrentActivity = e.CurrentActivity;
        }
    }
}

namespace Activity.UI
{
    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double opacity = 0.5;
            if (parameter != null && parameter is double)
                opacity = (double)parameter;

            return ((bool)value) ? 1 : opacity;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}