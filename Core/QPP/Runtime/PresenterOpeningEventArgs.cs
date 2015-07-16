using QPP.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QPP.Runtime
{
    public class PresenterOpeningEventArgs : EventArgs
    {
        public string Uri { get; set; }

        public string Key { get; set; }

        public DataArgs Args { get; set; }

        public PresenterOpeningEventArgs()
        {
        }

        public PresenterOpeningEventArgs(string uri, string key, DataArgs args)
        {
            Uri = uri;
            Key = key;
            Args = args;
        }
    }
}
