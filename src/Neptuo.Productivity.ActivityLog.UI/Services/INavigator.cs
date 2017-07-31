using Neptuo.Productivity.ActivityLog.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.Services
{
    public interface INavigator
    {
        Task Message(string message);
        Task Message(string title, string message);
        Task<bool> Confirm(string message);
        Task<bool> Confirm(string title, string message);

        Task TodayOverview();
        Task CategorySummary();
        Task<bool> Configuration();

        Task<ICategory> EditCategory(ICategory category);
        Task<ICategory> NewCategory();

        void Exit();
    }
}
