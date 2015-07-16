using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Command
{
    public class CommandEventArgs : EventArgs
    {
        public object Parameter { get; private set; }

        public CommandEventArgs(object parameter)
        {
            Parameter = parameter;
        }
    }
}
