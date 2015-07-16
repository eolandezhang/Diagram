using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QPP.Layout
{
    public class ContentCancelEventArgs : CancelEventArgs
    {
        public IDockingContent Content { get; private set; }

        public ContentCancelEventArgs(IDockingContent content)
        {
            Content = content;
        }

        public ContentCancelEventArgs(IDockingContent content, bool cancel)
            : base(cancel)
        {
            Content = content;
        }
    }
}
