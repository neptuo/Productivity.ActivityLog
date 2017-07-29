using Neptuo.Formatters;
using Neptuo.Productivity.ActivityLog.Data;
using Neptuo.Productivity.ActivityLog.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.Properties
{
    partial class Settings : ISettings
    {
        private CategoryListModel categories;

        public IReadOnlyList<ICategory> Categories
        {
            get
            {
                if (categories == null)
                {
                    categories = new CategoryListModel();
                    if (!string.IsNullOrEmpty(CategoriesRaw))
                    {
                        JsonCompositeStorage storage = new JsonCompositeStorage();
                        using (MemoryStream source = new MemoryStream(Encoding.UTF8.GetBytes(CategoriesRaw)))
                        {
                            source.Position = 0;
                            storage.Load(source);
                        }

                        categories.Load(storage);
                    }
                }

                return categories.Storage;
            }
            set
            {
                if (categories == null)
                    categories = new CategoryListModel();

                categories.Storage.Clear();

                if (value != null)
                {
                    categories.Storage.AddRange(value);

                    using (MemoryStream target = new MemoryStream())
                    {
                        JsonCompositeStorage storage = new JsonCompositeStorage();
                        categories.Save(storage);
                        storage.Store(target);
                        target.Position = 0;

                        CategoriesRaw = Encoding.UTF8.GetString(target.ToArray());
                    }
                }
            }
        }
    }
}
