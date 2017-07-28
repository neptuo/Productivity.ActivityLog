using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Neptuo.Productivity.ActivityLog.Services.Models
{
    public interface ICategory
    {
        Color Color { get; }
        string Name { get; }
    }
}
