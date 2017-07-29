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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DrawingColor = System.Drawing.Color;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Neptuo.Productivity.ActivityLog.Views
{
    /// <summary>
    /// Interaction logic for CategoryEdit.xaml
    /// </summary>
    public partial class CategoryEdit : Window, IViewModel<CategoryEditViewModel>
    {
        public CategoryEditViewModel ViewModel
        {
            get { return (CategoryEditViewModel)DataContext; }
        }

        public CategoryEdit(CategoryEditViewModel viewModel)
        {
            Ensure.NotNull(viewModel, "viewModel");
            DataContext = viewModel;

            InitializeComponent();
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
                e.Handled = true;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Color_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.Color = DrawingColor.FromArgb(ViewModel.Color.A, ViewModel.Color.G, ViewModel.Color.R, ViewModel.Color.B);
            DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ViewModel.Color = new Color()
                {
                    A = dialog.Color.A,
                    R = dialog.Color.R,
                    G = dialog.Color.G,
                    B = dialog.Color.B
                };
            }
        }
    }
}
