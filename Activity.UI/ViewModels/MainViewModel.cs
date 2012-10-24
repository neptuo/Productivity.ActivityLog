using Activity.Core;
using Activity.Core.Models;
using Neptuo.DesktopCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
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
            service.CurrentActivityChanged += (sender, e) =>
            {
                UpdateFilter(service);
                CurrentActivity = e.CurrentActivity;
            };
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(service.CurrentActivities);
            collectionView.Filter += (dataItem) => !(dataItem as ActivityModel).IsHidden;
            collectionView.SortDescriptions.Add(new SortDescription("UsedTime", ListSortDirection.Descending));
            UpdateFilter(service);
        }

        public void UpdateFilter(ActivityService service)
        {
            //service.CurrentActivities = service.CurrentActivities.OrderBy(m => m.Percentage);
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(service.CurrentActivities);
            collectionView.Refresh();
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

    public class InversedBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new BooleanToVisibilityConverter().Convert(!(bool)value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}