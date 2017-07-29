using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.Services
{
    public interface INavigationHandler
    {
        void SetResult();
    }

    public interface INavigationHandler<T> : INavigationHandler
    {
        void SetResult(T result);
    }
}
