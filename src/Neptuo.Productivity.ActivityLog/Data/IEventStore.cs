using Neptuo.Productivity.ActivityLog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.Data
{
    public interface IEventStore
    {
        Task SaveAsync(ActivityStarted payload);
        Task SaveAsync(ActivityEnded payload);
    }
}
