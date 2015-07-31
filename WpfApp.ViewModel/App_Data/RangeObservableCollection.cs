using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace WpfApp.ViewModel.App_Data
{
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        private bool surpressEvents = false;

        public void AddRange(IEnumerable<T> items)
        {
            surpressEvents = true;
            foreach (var item in items)
            {
                base.Add(item);
            }
            this.surpressEvents = false;
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items.ToList()));

        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!this.surpressEvents)
            {
                base.OnCollectionChanged(e);
            }
        }
    }
}
