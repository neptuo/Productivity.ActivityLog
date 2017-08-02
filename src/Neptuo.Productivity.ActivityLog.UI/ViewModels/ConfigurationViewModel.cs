using Neptuo.Observables;
using Neptuo.Productivity.ActivityLog.Properties;
using Neptuo.Productivity.ActivityLog.Services;
using Neptuo.Productivity.ActivityLog.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.ActivityLog.ViewModels
{
    public class ConfigurationViewModel : ObservableObject
    {
        public string Version { get; private set; }
        public CategoryListViewModel Categories { get; private set; }
        public ICommand Save { get; private set; }

        public ConfigurationViewModel(INavigator navigator, ISettings settings, INavigationContext<bool> handler)
        {
            Version = VersionInfo.Version + VersionInfo.Preview;
            Categories = new CategoryListViewModel(navigator);
            Save = new SaveConfigurationCommand(this, settings, handler);
        }
    }
}
