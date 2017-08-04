using Neptuo.Productivity.ActivityLog.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Productivity.ActivityLog.Services.Models;

namespace Neptuo.Productivity.ActivityLog.Views.DesignData
{
    public class Navigator : INavigator
    {
        public Task ApplicationSummary()
        {
            throw new NotImplementedException();
        }

        public Task CategorySummary()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Configuration()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Confirm(string message)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Confirm(string title, string message)
        {
            throw new NotImplementedException();
        }

        public Task<ICategory> EditCategory(ICategory category)
        {
            throw new NotImplementedException();
        }

        public void Exit()
        {
            throw new NotImplementedException();
        }

        public Task Message(string message)
        {
            throw new NotImplementedException();
        }

        public Task Message(string title, string message)
        {
            throw new NotImplementedException();
        }

        public Task<ICategory> NewCategory()
        {
            throw new NotImplementedException();
        }

        public Task TodayOverview()
        {
            throw new NotImplementedException();
        }
    }
}
