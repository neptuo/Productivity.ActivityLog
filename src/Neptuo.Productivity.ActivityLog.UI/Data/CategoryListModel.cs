using Neptuo.Collections.Specialized;
using Neptuo.Formatters;
using Neptuo.Productivity.ActivityLog.Services.Models;
using Neptuo.Productivity.ActivityLog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Neptuo.Productivity.ActivityLog.Data
{
    public class CategoryListModel : ICompositeModel
    {
        public List<ICategory> Storage { get; } = new List<ICategory>();

        public void Load(ICompositeStorage storage)
        {
            Storage.Clear();

            int count = storage.Get<int>("Count");
            for (int i = 0; i < count; i++)
            {
                if (storage.TryGet(i.ToString(), out ICompositeStorage categoryStorage))
                    Storage.Add(LoadCategory(categoryStorage));
            }
        }

        private ICategory LoadCategory(ICompositeStorage storage)
        {
            CategoryViewModel item = new CategoryViewModel();
            item.Name = storage.Get<string>("Name");
            item.Color = new Color()
            {
                A = (byte)storage.Get<int>("ColorA"),
                R = (byte)storage.Get<int>("ColorR"),
                G = (byte)storage.Get<int>("ColorG"),
                B = (byte)storage.Get<int>("ColorB"),
            };

            if (storage.TryGet("Rules", out ICompositeStorage rulesStorage))
            {
                int count = rulesStorage.Get<int>("Count");
                for (int i = 0; i < count; i++)
                {
                    if (rulesStorage.TryGet(i.ToString(), out ICompositeStorage ruleStorage))
                        item.Rules.Add(LoadRule(ruleStorage));
                }
            }

            return item;
        }

        private RuleViewModel LoadRule(ICompositeStorage storage)
        {
            RuleViewModel item = new RuleViewModel();
            item.ApplicationPath = storage.Get<string>("ApplicationPath");
            item.WindowTitle = storage.Get<string>("WindowTitle");
            return item;
        }

        public void Save(ICompositeStorage storage)
        {
            storage.Add("Count", Storage.Count);
            for (int i = 0; i < Storage.Count; i++)
            {
                ICategory category = Storage[i];
                ICompositeStorage categoryStorage = storage.Add(i.ToString());
                SaveCategory(category, categoryStorage);
            }
        }

        private void SaveCategory(ICategory item, ICompositeStorage storage)
        {
            storage.Add("Name", item.Name);
            storage.Add("ColorA", (int)item.Color.A);
            storage.Add("ColorR", (int)item.Color.R);
            storage.Add("ColorG", (int)item.Color.G);
            storage.Add("ColorB", (int)item.Color.B);

            ICompositeStorage rulesStorage = storage.Add("Rules");
            rulesStorage.Add("Count", item.Rules.Count);

            for (int i = 0; i < item.Rules.Count; i++)
            {
                IRule rule = item.Rules[i];
                ICompositeStorage ruleStorage = rulesStorage.Add(i.ToString());
                SaveRule(rule, ruleStorage);
            }
        }

        private void SaveRule(IRule item, ICompositeStorage storage)
        {
            storage.Add("ApplicationPath", item.ApplicationPath);
            storage.Add("WindowTitle", item.WindowTitle);
        }
    }
}
