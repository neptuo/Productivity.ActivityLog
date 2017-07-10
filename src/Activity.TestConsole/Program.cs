using Activity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Activity.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            ActivityProvider service = new ActivityProvider();
            service.WindowChanged += (sender, e) =>
            {
                Console.WriteLine(e.CurrentProcess.MainModule.FileName);
            };
            service.Start();
        }
    }
}
