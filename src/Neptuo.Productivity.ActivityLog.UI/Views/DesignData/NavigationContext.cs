using Neptuo.Productivity.ActivityLog.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.Views.DesignData
{
    internal class NavigationContext<T> : INavigationContext<T>
    {
        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Close(T result)
        {
            throw new NotImplementedException();
        }
    }

    internal class NavigationContext : NavigationContext<object>
    { }
}
