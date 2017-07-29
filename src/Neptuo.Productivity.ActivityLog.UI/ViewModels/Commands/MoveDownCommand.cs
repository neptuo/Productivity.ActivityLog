using Neptuo.Observables.Collections;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.ActivityLog.ViewModels.Commands
{
    public class MoveDownCommand<T> : Command<T>
    {
        private readonly ObservableCollection<T> source;

        public MoveDownCommand(ObservableCollection<T> source)
        {
            Ensure.NotNull(source, "source");
            this.source = source;
            this.source.CollectionChanged += OnSourceChanged;
        }

        private void OnSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaiseCanExecuteChanged();
        }

        public override bool CanExecute(T parameter)
        {
            if (parameter == null)
                return false;

            int index = source.IndexOf(parameter);
            return index < source.Count - 1;
        }

        public override void Execute(T parameter)
        {
            int index = source.IndexOf(parameter);
            source.Move(index, index + 1);
        }
    }
}
