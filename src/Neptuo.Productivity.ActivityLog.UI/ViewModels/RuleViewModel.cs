using Neptuo.Observables;
using Neptuo.Productivity.ActivityLog.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.ViewModels
{
    public class RuleViewModel : ObservableObject, IRule
    {
        private string windowTitle;
        public string WindowTitle
        {
            get { return windowTitle; }
            set
            {
                if (windowTitle != value)
                {
                    windowTitle = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string applicationPath;
        public string ApplicationPath
        {
            get { return applicationPath; }
            set
            {
                if (applicationPath != value)
                {
                    applicationPath = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
