using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QPP.ComponentModel;
using System.ComponentModel;
using QPP.Threading;

namespace QPP.Wpf.Threading
{
    public class ProgressItem : StatefulObject
    {
        public WorkItem Worker { get; private set; }

        public ProgressItem(WorkItem item)
        {
            Worker = item;
            Name = item.Name;
            IsIndeterminate = !Worker.WorkerReportsProgress;
            CanCancel = Worker.WorkerSupportsCancellation;
            Worker.ProgressChanged += (s, e) =>
            {
                Progress = e.ProgressPercentage;
                Message = e.UserState.ToSafeString();
            };
            Worker.RunWorkerCompleted += (s, e) => { CanCancel = true; };
        }

        public bool CanCancel
        {
            get { return Get<bool>("CanCancel"); }
            set { Set("CanCancel", value); }
        }
        public bool HasError
        {
            get { return Get<bool>("HasError"); }
            set { Set("HasError", value); }
        }
        public string Name
        {
            get { return Get<string>("Name"); }
            set { Set("Name", value); }
        }
        public int Progress
        {
            get { return Get<int>("Progress"); }
            set { Set("Progress", value); }
        }
        public string Message
        {
            get { return Get<string>("Message"); }
            set { Set("Message", value); }
        }
        public string Details
        {
            get { return Get<string>("Details"); }
            set { Set("Details", value); }
        }
        public bool IsIndeterminate
        {
            get { return Get<bool>("IsIndeterminate"); }
            set { Set("IsIndeterminate", value); }
        }
    }
}
