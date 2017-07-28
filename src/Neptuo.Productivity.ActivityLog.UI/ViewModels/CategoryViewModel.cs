using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.ActivityLog.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Neptuo.Productivity.ActivityLog.ViewModels
{
    public class CategoryViewModel : ObservableObject, ICategory
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }

        private Color color;
        public Color Color
        {
            get { return color; }
            set
            {
                if (color != value)
                {
                    color = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<RuleViewModel> Rules { get; private set; }

        IReadOnlyList<IRule> ICategory.Rules => Rules;

        public CategoryViewModel()
        {
            Rules = new ObservableCollection<RuleViewModel>();
        }
    }
}
