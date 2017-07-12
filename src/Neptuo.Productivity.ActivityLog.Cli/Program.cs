using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Productivity.ActivityLog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.Cli
{
    class Program : IEventHandler<ActivityStarted>, IEventHandler<ActivityEnded>
    {
        static void Main(string[] args)
        {
            Program program = new Program();

            DefaultEventManager eventManager = new DefaultEventManager();
            eventManager.AddAll(program);

            DomainService service = new DomainService(eventManager);

            while (true)
            {
                Console.WriteLine("Press Esc to exit...");
                WriteSeparator();
                ConsoleKeyInfo input = Console.ReadKey(true);
                if (input.Key == ConsoleKey.Escape)
                    break;
            }

            service.Dispose();
            Console.WriteLine("Exiting now...");
            WriteSeparator();
        }

        public Task HandleAsync(ActivityStarted payload)
        {
            Console.WriteLine($"App: {Path.GetFileName(payload.ApplicationPath)}");
            Console.WriteLine($"Title: {payload.WindowTitle}");
            Console.WriteLine($"StartedAt: {payload.StartedAt.ToLongTimeString()}");
            return Task.CompletedTask;
        }

        public Task HandleAsync(ActivityEnded payload)
        {
            Console.WriteLine($"EndedAt: {payload.EndedAt.ToLongTimeString()}");
            WriteSeparator();
            return Task.CompletedTask;
        }

        private static void WriteSeparator()
        {
            Console.WriteLine("----------------------------------------");
        }
    }
}
