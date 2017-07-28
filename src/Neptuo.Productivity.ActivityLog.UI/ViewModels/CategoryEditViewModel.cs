using Neptuo;
using Neptuo.Observables;
using Neptuo.Observables.Commands;
using Neptuo.Productivity.ActivityLog.Services.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.ActivityLog.ViewModels
{
    public class CategoryEditViewModel : CategoryViewModel
    {
        public ICommand CreateRule { get; private set; }
        public ICommand RemoveRule { get; private set; }
        public ICommand Save { get; private set; }

        public CategoryEditViewModel(Action onSave)
        {
            Ensure.NotNull(onSave, "onSave");
            CreateRule = new DelegateCommand(() => Rules.Add(new RuleViewModel()));
            RemoveRule = new DelegateCommand<RuleViewModel>(vm => Rules.Remove(vm));
            Save = new SaveCommand(this, onSave);
        }

        private class SaveCommand : Command
        {
            private readonly CategoryEditViewModel viewModel;
            private readonly Action onSave;

            public SaveCommand(CategoryEditViewModel viewModel, Action onSave)
            {
                this.viewModel = viewModel;
                this.onSave = onSave;

                viewModel.PropertyChanged += OnPropertyChanged;
            }

            private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(Name))
                    RaiseCanExecuteChanged();
            }

            public override bool CanExecute()
            {
                return !string.IsNullOrEmpty(viewModel.Name);
            }

            public override void Execute()
            {
                onSave();
            }
        }
    }
}
