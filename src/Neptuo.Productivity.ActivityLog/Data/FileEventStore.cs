using Neptuo;
using Neptuo.Formatters;
using Neptuo.Productivity.ActivityLog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.Data
{
    public class FileEventStore : IEventStore
    {
        private readonly ISerializer serializer;
        private readonly Func<DateTime, string> fileNameProvider;

        public FileEventStore(ISerializer serializer, Func<DateTime, string> fileNameProvider)
        {
            Ensure.NotNull(serializer, "serializer");
            Ensure.NotNull(fileNameProvider, "fileNameProvider");
            this.serializer = serializer;
            this.fileNameProvider = fileNameProvider;
        }

        public Task SaveAsync(ActivityStarted payload)
        {
            return SaveAsync(payload, typeof(ActivityStarted), payload.StartedAt);
        }

        public Task SaveAsync(ActivityEnded payload)
        {
            return SaveAsync(payload, typeof(ActivityEnded), payload.EndedAt);
        }

        private Task SaveAsync(object payload, Type payloadType, DateTime dateTime)
        {
            string fileName = fileNameProvider(dateTime);
            using (StreamWriter writer = File.AppendText(fileName))
                return serializer.TrySerializeAsync(payload, new DefaultSerializerContext(payloadType, writer.BaseStream));
        }
    }
}
