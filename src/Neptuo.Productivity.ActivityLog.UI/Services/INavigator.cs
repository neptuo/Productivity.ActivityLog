using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.Services
{
    public interface INavigator
    {
        void Message(string message);
        void Message(string title, string message);
        bool Confirm(string message);
        bool Confirm(string title, string message);

        void Overview();

        void Exist();
    }
}
