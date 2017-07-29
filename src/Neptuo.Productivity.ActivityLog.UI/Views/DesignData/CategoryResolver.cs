using Neptuo.Productivity.ActivityLog.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Productivity.ActivityLog.Services.Models;

namespace Neptuo.Productivity.ActivityLog.Views.DesignData
{
    internal class CategoryResolver : ICategoryResolver
    {
        public Dictionary<string, ICategory> ApplicationPathToCategory { get; private set; } = new Dictionary<string, ICategory>();

        public ICategory TryResolve(string applicationPath, string windowTitle)
        {
            if (ApplicationPathToCategory.TryGetValue(applicationPath, out ICategory category))
                return category;

            return null;
        }
    }
}
