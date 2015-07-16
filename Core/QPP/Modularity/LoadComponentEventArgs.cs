using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Modularity
{
    public class LoadComponentEventArgs : EventArgs
    {
        public Uri Uri { get; private set; }

        public LoadComponentEventArgs(Uri uri)
        {
            Uri = uri;
        }
    }
}
