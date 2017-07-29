using Neptuo;
using Neptuo.Productivity.ActivityLog.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog
{
    public class TaskCompletionSourceNavigationHandler : TaskCompletionSourceNavigationHandler<object>
    {
        public TaskCompletionSourceNavigationHandler(TaskCompletionSource<object> source) 
            : base(source)
        { }
    }

    public class TaskCompletionSourceNavigationHandler<T> : INavigationHandler<T>
    {
        private readonly TaskCompletionSource<T> source;

        public TaskCompletionSourceNavigationHandler(TaskCompletionSource<T> source)
        {
            Ensure.NotNull(source, "source");
            this.source = source;
        }

        public void SetResult(T result)
        {
            source.SetResult(result);
        }

        public void SetResult()
        {
            source.SetResult(default(T));
        }
    }
}
