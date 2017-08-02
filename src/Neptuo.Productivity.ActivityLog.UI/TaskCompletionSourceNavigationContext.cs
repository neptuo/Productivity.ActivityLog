using Neptuo;
using Neptuo.Productivity.ActivityLog.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog
{
    public class TaskCompletionSourceNavigationContext : TaskCompletionSourceNavigationHandler<object>
    {
        public TaskCompletionSourceNavigationContext(TaskCompletionSource<object> source) 
            : base(source)
        { }
    }

    public class TaskCompletionSourceNavigationHandler<T> : INavigationContext<T>
    {
        private readonly TaskCompletionSource<T> source;

        public TaskCompletionSourceNavigationHandler(TaskCompletionSource<T> source)
        {
            Ensure.NotNull(source, "source");
            this.source = source;
        }

        public void Close(T result)
        {
            source.SetResult(result);
        }

        public void Close()
        {
            source.SetResult(default(T));
        }
    }
}
