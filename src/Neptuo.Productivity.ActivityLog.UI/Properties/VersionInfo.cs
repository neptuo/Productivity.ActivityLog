using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

namespace Neptuo.Productivity.ActivityLog
{
    public class VersionInfo
    {
        internal const string Version = "1.0.0";
        internal const string Preview = "-beta1";

        public static Version GetVersion()
        {
            return new Version(Version);
        }
    }
}