using Neptuo;
using Neptuo.Productivity.ActivityLog.ViewModels;
using Neptuo.Productivity.ActivityLog.Views.Controls;
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
    /// Interaction logic for TodayCategory.xaml
    /// </summary>
    public partial class CategorySummary : Window, IViewModel<CategorySummaryViewModel>
    {
        public CategorySummaryViewModel ViewModel
        {
            get { return (CategorySummaryViewModel)DataContext; }
        }

        public CategorySummary(CategorySummaryViewModel viewModel)
        {
            Ensure.NotNull(viewModel, "viewModel");
            InitializeComponent();

            DataContext = viewModel;
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
    }
}
