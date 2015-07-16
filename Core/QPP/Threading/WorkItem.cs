using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QPP.ComponentModel;
using System.ComponentModel;

namespace QPP.Threading
{
    public class WorkItem : BackgroundWorker
    {
        public WorkItem(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
