using Neptuo.Observables.Collections;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace Neptuo.Productivity.ActivityLog.ViewModels.Commands
{
    public class MoveUpCommand<T> : Command<T>
    {
        private readonly ObservableCollection<T> source;

        public MoveUpCommand(ObservableCollection<T> source)
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
            return index > 0;
        }

        public override void Execute(T parameter)
        {
            int index = source.IndexOf(parameter);
            source.Move(index, index - 1);
        }
    }
}
