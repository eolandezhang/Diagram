using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Layout
{
    public class ContentEventArgs : EventArgs
    {
        public IDockingContent Content { get; private set; }

        public ContentEventArgs(IDockingContent content)
        {
            Content = content;
        }
    }
}
