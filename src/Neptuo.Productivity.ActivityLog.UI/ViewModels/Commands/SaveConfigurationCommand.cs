using Neptuo.Observables.Commands;
using Neptuo.Productivity.ActivityLog.Properties;
using Neptuo.Productivity.ActivityLog.Services;
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
        private readonly INavigationContext<bool> handler;

        public SaveConfigurationCommand(ConfigurationViewModel viewModel, ISettings settings, INavigationContext<bool> handler)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(handler, "handler");
            this.viewModel = viewModel;
            this.settings = settings;
            this.handler = handler;
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override void Execute()
        {
            settings.Categories = viewModel.Categories.Items;
            settings.Save();

            handler.Close(true);
        }
    }
}
