using Neptuo;
using Neptuo.Observables.Commands;
using Neptuo.Productivity.ActivityLog.Services;
using Neptuo.Productivity.ActivityLog.Services.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.ViewModels.Commands
{
    public class SaveCategoryEditCommand : Command, IDisposable
    {
        private readonly CategoryEditViewModel viewModel;
        private readonly INavigationContext<ICategory> handler;

        public SaveCategoryEditCommand(CategoryEditViewModel viewModel, INavigationContext<ICategory> handler)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(handler, "handler");
            this.viewModel = viewModel;
            this.viewModel.PropertyChanged += OnPropertyChanged;
            this.handler = handler;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CategoryEditViewModel.Name))
                RaiseCanExecuteChanged();
        }

        public override bool CanExecute()
        {
            return !string.IsNullOrEmpty(viewModel.Name);
        }

        public override void Execute()
        {
            handler.Close(viewModel);
        }

        public void Dispose()
        {
            viewModel.PropertyChanged -= OnPropertyChanged;
        }
    }
}
