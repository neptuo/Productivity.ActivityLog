using Neptuo;
using Neptuo.Productivity.ActivityLog.ViewModels;
using Neptuo.Productivity.ActivityLog.Views.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Neptuo.Windows.Threading;
using Neptuo.Productivity.ActivityLog.Services;

namespace Neptuo.Productivity.ActivityLog.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TodayOverview : Window, IViewModel<TodayOverviewViewModel>
    {
        private readonly INavigator navigator;
        private readonly DispatcherHelper dispatcher;

        public TodayOverviewViewModel ViewModel
        {
            get { return (TodayOverviewViewModel)DataContext; }
        }

        public TodayOverview(TodayOverviewViewModel viewModel, INavigator navigator)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(navigator, "navigator");
            this.navigator = navigator;

            InitializeComponent();

            dispatcher = new DispatcherHelper(Dispatcher);

            DataContext = viewModel;
            if (viewModel.Activities is INotifyCollectionChanged collection)
            {
                collection.CollectionChanged += OnActivitiesChanged;

                foreach (ActivityOverviewViewModel item in viewModel.Activities)
                    item.PropertyChanged += OnActivityChanged;
            }
        }

        private void OnActivitiesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ActivityOverviewViewModel item in e.NewItems)
                        item.PropertyChanged += OnActivityChanged;

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (ActivityOverviewViewModel item in e.OldItems)
                        item.PropertyChanged -= OnActivityChanged;

                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (ActivityOverviewViewModel item in e.NewItems)
                        item.PropertyChanged += OnActivityChanged;

                    break;
            }
        }

        private void OnActivityChanged(object sender, PropertyChangedEventArgs e)
        {
            dispatcher.Run(() =>
            {
                if (e.PropertyName == nameof(ActivityOverviewViewModel.Duration))
                {
                    CollectionViewSource source = (CollectionViewSource)FindResource("ActivitiesCollection");
                    source.View.Refresh();
                }
            });
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowDrag.TryMove(e))
                DragMove();
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
                e.Handled = true;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Configuration_Click(object sender, RoutedEventArgs e)
        {
            navigator.Configuration();
        }
    }
}
