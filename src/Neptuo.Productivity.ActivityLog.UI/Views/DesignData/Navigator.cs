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
        public void Configuration()
        {
            throw new NotImplementedException();
        }

        public bool Confirm(string message)
        {
            throw new NotImplementedException();
        }

        public bool Confirm(string title, string message)
        {
            throw new NotImplementedException();
        }

        public Task<ICategory> EditCategory(ICategory category)
        {
            throw new NotImplementedException();
        }

        public void Exist()
        {
            throw new NotImplementedException();
        }

        public void Message(string message)
        {
            throw new NotImplementedException();
        }

        public void Message(string title, string message)
        {
            throw new NotImplementedException();
        }

        public Task<ICategory> NewCategory()
        {
            throw new NotImplementedException();
        }

        public void TodayOverview()
        {
            throw new NotImplementedException();
        }
    }
}
