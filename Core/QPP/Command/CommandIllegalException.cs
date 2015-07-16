using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Command
{
    public class CommandIllegalException : AppException
    {
        public CommandIllegalException(string message)
            : base(message)
        {

        }
    }
}
