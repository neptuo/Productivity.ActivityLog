using Neptuo;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Formatters;
using Neptuo.Productivity.ActivityLog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Neptuo.Productivity.ActivityLog.Data
{
    public class RecoveryService : IEventHandler<ActivityStarted>, IEventHandler<ActivityEnded>
    {
        private readonly IFormatter formatter;
        private readonly Func<string> fileNameProvider;
        private readonly Timer timer;

        private ActivityStarted lastPayload;

        public RecoveryService(IFormatter formatter, Func<string> fileNameProvider)
        {
            Ensure.NotNull(formatter, "formatter");
            Ensure.NotNull(fileNameProvider, "fileNameProvider");
            this.formatter = formatter;
            this.fileNameProvider = fileNameProvider;

            timer = new Timer(60 * 1000);
            timer.Elapsed += OnTick;
            timer.Start();
        }

        Task IEventHandler<ActivityStarted>.HandleAsync(ActivityStarted payload)
        {
            lastPayload = payload;
            TrySaveLastPayload();
            return Task.CompletedTask;
        }

        Task IEventHandler<ActivityEnded>.HandleAsync(ActivityEnded payload)
        {
            lastPayload = null;
            return Task.CompletedTask;
        }

        private void OnTick(object sender, ElapsedEventArgs e)
        {
            TrySaveLastPayload();
        }

        private void TrySaveLastPayload()
        {
            if (lastPayload != null)
            {
                string fileName = fileNameProvider();
                using (Stream stream = File.Open(fileName, FileMode.Create))
                {
                    ActivityEnded payload = new ActivityEnded(lastPayload.ApplicationPath, lastPayload.WindowTitle, DateTime.Now);
                    ISerializerContext context = new DefaultSerializerContext(lastPayload.GetType(), stream);
                    formatter.TrySerialize(payload, context);
                }
            }
        }

        public async Task RecoverAsync(IEventDispatcher eventDispatcher)
        {
            string fileName = fileNameProvider();
            if (File.Exists(fileName))
            {
                using (Stream stream = File.OpenRead(fileName))
                {
                    IDeserializerContext context = new DefaultDeserializerContext(typeof(IEnumerable<IEvent>));
                    if (formatter.TryDeserialize(stream, context))
                    {
                        IEvent payload = ((IEnumerable<IEvent>)context.Output).LastOrDefault();
                        if (payload is ActivityEnded ended)
                            await eventDispatcher.PublishAsync(ended);
                    }
                }

                File.Delete(fileName);
            }
        }
    }
}
