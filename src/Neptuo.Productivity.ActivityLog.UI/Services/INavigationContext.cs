using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.Services
{
    public interface INavigationContext
    {
        void Close();
    }

    public interface INavigationContext<T> : INavigationContext
    {
        void Close(T result);
    }
}
