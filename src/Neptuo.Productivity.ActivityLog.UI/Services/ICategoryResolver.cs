using Neptuo.Productivity.ActivityLog.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.Services
{
    public interface ICategoryResolver
    {
        ICategory TryResolve(string applicationPath, string windowTitle);
    }
}
