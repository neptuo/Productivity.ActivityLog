using Neptuo;
using Neptuo.Productivity.ActivityLog.ViewModels;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for ApplicationSummary.xaml
    /// </summary>
    public partial class ApplicationSummary : Window, IView<ApplicationSummaryViewModel>
    {
        public ApplicationSummaryViewModel ViewModel
        {
            get { return (ApplicationSummaryViewModel)DataContext; }
        }

        public ApplicationSummary(ApplicationSummaryViewModel viewModel)
        {
            Ensure.NotNull(viewModel, "viewModel");
            InitializeComponent();

            DataContext = viewModel;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                Close();
            }
        }
    }
}
