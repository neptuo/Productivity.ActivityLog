﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.Services
{
    public interface IApplicationNameProvider
    {
        string GetName(string applicationPath);
    }
}
