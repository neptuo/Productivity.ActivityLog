using Neptuo;
using Neptuo.Exceptions.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Neptuo.Productivity.ActivityLog.Services.Exceptions
{
    public class MessageExceptionHandler : IExceptionHandler<Exception>
    {
        private readonly INavigator navigator;

        public MessageExceptionHandler(INavigator navigator)
        {
            Ensure.NotNull(navigator, "navigator");
            this.navigator = navigator;
        }

        public async void Handle(Exception exception)
        {
            StringBuilder message = new StringBuilder();

            string exceptionMessage = exception.ToString();
            if (exceptionMessage.Length > 800)
                exceptionMessage = exceptionMessage.Substring(0, 800);

            message.AppendLine(exceptionMessage);
            
            if (await navigator.Confirm("Do you want to kill the aplication?", message.ToString()))
                navigator.Exit();
        }
    }
}
