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

namespace Neptuo.Productivity.ActivityLog.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherHelper dispatcher;

        public OverviewViewModel ViewModel
        {
            get { return (OverviewViewModel)DataContext; }
        }

        public MainWindow(OverviewViewModel viewModel)
        {
            Ensure.NotNull(viewModel, "viewModel");
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
                Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
