using Neptuo.Events.Handlers;
using Neptuo.Productivity.ActivityLog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.Data
{
    public class EventStoreHandler : IEventHandler<ActivityStarted>, IEventHandler<ActivityEnded>
    {
        private readonly IEventStore store;

        public EventStoreHandler(IEventStore store)
        {
            Ensure.NotNull(store, "store");
            this.store = store;
        }

        public Task HandleAsync(ActivityStarted payload)
        {
            return store.SaveAsync(payload);
        }

        public Task HandleAsync(ActivityEnded payload)
        {
            return store.SaveAsync(payload);
        }
    }
}
