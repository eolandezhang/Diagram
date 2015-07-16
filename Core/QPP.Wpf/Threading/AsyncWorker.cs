using QPP.ComponentModel;
using QPP.Messaging;
using QPP.Threading;
using QPP.Wpf.Controls.Progress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Threading
{
    public class AsyncWorker : IAsyncWorker
    {
        List<WorkItem> items = new List<WorkItem>();

        public IEnumerable<WorkItem> Items { get { return items; } }

        public void Run(WorkItem item)
        {
            items.Add(item);
            item.RunWorkerCompleted += (x, y) => { items.Remove(item); };
            Progresser.Default.AddItem(new ProgressItem(item));
            RuntimeContainers.Current.OpenContent<Progresser>();
            if (!item.IsBusy)
                item.RunWorkerAsync();
        }
    }
}
