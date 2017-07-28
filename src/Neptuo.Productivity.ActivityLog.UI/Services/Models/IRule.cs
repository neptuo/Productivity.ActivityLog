using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.Services.Models
{
    public interface IRule
    {
        string WindowTitle { get; }
        string ApplicationPath { get; }
    }
}
