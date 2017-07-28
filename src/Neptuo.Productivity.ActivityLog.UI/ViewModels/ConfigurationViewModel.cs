using Neptuo.Observables;
using Neptuo.Productivity.ActivityLog.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.ViewModels
{
    public class ConfigurationViewModel : ObservableObject
    {
        public string Version { get; private set; }
        public CategoryListViewModel Categories { get; private set; }

        public ConfigurationViewModel(INavigator navigator)
        {
            Version = VersionInfo.Version + VersionInfo.Preview;
            Categories = new CategoryListViewModel(navigator);
        }
    }
}
