using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QPP.Command
{
    public class CancelCommandEventArgs : CancelEventArgs
    {
        public object Parameter { get; private set; }

        public CancelCommandEventArgs() { }

        public CancelCommandEventArgs(object parameter)
        {
            Parameter = parameter;
        }
    }
}
