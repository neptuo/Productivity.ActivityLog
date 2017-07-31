using Neptuo;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Formatters;
using Neptuo.Productivity.ActivityLog.Events;
using Neptuo.Productivity.ActivityLog.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog
{
    public class ApplicationHistoryApplier : IHistoryApplier
    {
        private readonly IDeserializer deserializer;
        private readonly Func<DateTime, string> eventStoreFileNameGetter;

        public ApplicationHistoryApplier(IDeserializer deserializer, Func<DateTime, string> eventStoreFileNameGetter)
        {
            Ensure.NotNull(deserializer, "deserializer");
            Ensure.NotNull(eventStoreFileNameGetter, "eventStoreFileNameGetter");
            this.deserializer = deserializer;
            this.eventStoreFileNameGetter = eventStoreFileNameGetter;
        }

        public Task Apply(object handler, DateTime day)
        {
            string todayFile = eventStoreFileNameGetter(day);
            if (File.Exists(todayFile))
            {
                using (Stream file = File.OpenRead(todayFile))
                {
                    IDeserializerContext context = new DefaultDeserializerContext(typeof(IEnumerable<IEvent>));
                    if (deserializer.TryDeserialize(file, context))
                    {
                        foreach (IEvent output in (IEnumerable<IEvent>)context.Output)
                        {
                            if (output is ActivityStarted started && handler is IEventHandler<ActivityStarted> startedHandler)
                                startedHandler.HandleAsync(started).Wait();
                            else if (output is ActivityEnded ended && handler is IEventHandler<ActivityEnded> endedHandler)
                                endedHandler.HandleAsync(ended).Wait();
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        public async Task Apply(object handler, DateTime dateFrom, DateTime dateTo)
        {
            dateFrom = dateFrom.Date;
            dateTo = dateTo.Date;

            if (dateFrom < dateTo)
            {
                do
                {
                    await Apply(handler, dateFrom);
                    dateFrom = dateFrom.AddDays(1);
                } while (dateFrom <= dateTo);
            }
        }
    }
}
