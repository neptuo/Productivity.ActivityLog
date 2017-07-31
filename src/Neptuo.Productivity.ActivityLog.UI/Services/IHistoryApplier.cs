using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.Services
{
    public interface IHistoryApplier
    {
        Task Apply(object handler, DateTime date);

        Task Apply(object handler, DateTime dateFrom, DateTime dateTo);
    }
}
