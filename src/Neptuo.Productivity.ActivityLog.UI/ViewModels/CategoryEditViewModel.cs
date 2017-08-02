using Neptuo.Observables.Commands;
using Neptuo.Productivity.ActivityLog.Services;
using Neptuo.Productivity.ActivityLog.Services.Models;
using Neptuo.Productivity.ActivityLog.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.ActivityLog.ViewModels
{
    public class CategoryEditViewModel : CategoryViewModel, IDisposable
    {
        public ICommand CreateRule { get; private set; }
        public ICommand RemoveRule { get; private set; }
        public ICommand Save { get; private set; }

        public CategoryEditViewModel(INavigationContext<ICategory> handler)
        {
            CreateRule = new DelegateCommand(() => Rules.Add(new RuleViewModel()
            {
                ApplicationPath = "*",
                WindowTitle = "*"
            }));
            RemoveRule = new DelegateCommand<RuleViewModel>(vm => Rules.Remove(vm));
            Save = new SaveCategoryEditCommand(this, handler);
        }

        public void Dispose()
        {
            if (Save is IDisposable disposable)
                disposable.Dispose();
        }
    }
}
