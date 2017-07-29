using Neptuo.Productivity.ActivityLog.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.Views.DesignData
{
    internal class NavigationHandler<T> : INavigationHandler<T>
    {
        public void SetResult()
        {
            throw new NotImplementedException();
        }

        public void SetResult(T result)
        {
            throw new NotImplementedException();
        }
    }

    internal class NavigationHandler : NavigationHandler<object>
    { }
}
