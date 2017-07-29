using Neptuo.Productivity.ActivityLog.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.Properties
{
    public interface ISettings
    {
        double MainWindowTop { get; }
        double MainWindowLeft { get; }
        IReadOnlyList<ICategory> Categories { get; set; }

        void Save();
    }
}
