using Neptuo;
using Neptuo.Observables.Commands;
using Neptuo.Productivity.ActivityLog.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.ViewModels.Commands
{
    public class SaveConfigurationCommand : Command
    {
        private readonly ConfigurationViewModel viewModel;
        private readonly ISettings settings;

        internal SaveConfigurationCommand(ConfigurationViewModel viewModel, ISettings settings)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(settings, "settings");
            this.viewModel = viewModel;
            this.settings = settings;
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override void Execute()
        {
            settings.Categories = viewModel.Categories.Items;
            settings.Save();

            // TODO: Close configuration window.
        }
    }
}
