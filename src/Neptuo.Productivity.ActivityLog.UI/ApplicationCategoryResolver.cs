using Neptuo.Productivity.ActivityLog.Properties;
using Neptuo.Productivity.ActivityLog.Services;
using Neptuo.Productivity.ActivityLog.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog
{
    public class ApplicationCategoryResolver : ICategoryResolver
    {
        private readonly ISettings settings;

        public ApplicationCategoryResolver(ISettings settings)
        {
            Ensure.NotNull(settings, "settings");
            this.settings = settings;
        }

        public ICategory TryResolve(string applicationPath, string windowTitle)
        {
            foreach (ICategory category in settings.Categories)
            {
                foreach (IRule rule in category.Rules)
                {
                    if (IsMatch(applicationPath, rule.ApplicationPath) && IsMatch(windowTitle, rule.WindowTitle))
                        return category;
                }
            }

            return null;
        }

        public static bool IsMatch(string value, string pattern)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (string.IsNullOrEmpty(pattern))
                    return false;

                Regex regex = new Regex("^" + pattern.Replace("*", ".*?") + "$");
                if (!regex.IsMatch(value))
                    return false;
            }

            return true;
        }
    }
}
