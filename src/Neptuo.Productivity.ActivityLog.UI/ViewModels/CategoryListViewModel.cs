using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.ActivityLog.ViewModels
{
    public class CategoryListViewModel : ObservableObject
    {
        public ObservableCollection<CategoryEditViewModel> Items { get; private set; }

        public ICommand Create { get; private set; }
        public ICommand Remove { get; private set; }

        public CategoryListViewModel()
        {
            Items = new ObservableCollection<CategoryEditViewModel>();
            Create = new DelegateCommand(() => { });
            Remove = new DelegateCommand<CategoryEditViewModel>(vm => Items.Remove(vm));
        }
    }
}
