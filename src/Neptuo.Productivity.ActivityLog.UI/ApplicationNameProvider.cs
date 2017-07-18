using Neptuo.Productivity.ActivityLog.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog
{
    public class ApplicationNameProvider : IApplicationNameProvider
    {
        public string GetName(string applicationPath)
        {
            string applicationName = null;

            if (applicationPath != String.Empty)
            {
                FileVersionInfo applicationInfo = FileVersionInfo.GetVersionInfo(applicationPath);
                if (!string.IsNullOrEmpty(applicationInfo.FileDescription))
                {
                    applicationName = applicationInfo.FileDescription;
                }
                else
                {
                    applicationName = Path.GetFileNameWithoutExtension(applicationPath);
                    if (applicationName.Length > 0)
                    {
                        string first = applicationName.Substring(0, 1).ToUpper();
                        if (applicationName.Length > 1)
                        {
                            string remaining = applicationName.Substring(0);
                            applicationName = first + remaining;
                        }
                        else
                        {
                            applicationName = first;
                        }
                    }
                }

            }

            return applicationName;
        }
    }
}
