using Neptuo.Productivity.ActivityLog.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.Views.DesignData
{
    internal class Synchronizer : ISynchronizer
    {
        public void Run(Action handler)
        {
            handler();
        }
    }
}
