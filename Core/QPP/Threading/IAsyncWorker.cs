using QPP.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Threading
{
    public interface IAsyncWorker
    {
        void Run(WorkItem item);
        IEnumerable<WorkItem> Items { get; }
    }
}
