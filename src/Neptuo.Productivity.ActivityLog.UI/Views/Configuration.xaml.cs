using Neptuo;
using Neptuo.Productivity.ActivityLog.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace Neptuo.Productivity.ActivityLog.Views
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class Configuration : Window, IViewModel<ConfigurationViewModel>
    {
        public ConfigurationViewModel ViewModel
        {
            get { return (ConfigurationViewModel)DataContext; }
        }

        public Configuration(ConfigurationViewModel viewModel)
        {
            Ensure.NotNull(viewModel, "viewModel");
            DataContext = viewModel;

            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            btnErrorLog.IsEnabled = false;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
                e.Handled = true;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnErrorLog_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.neptuo.com/project/desktop/activitylog");
        }
    }
}
