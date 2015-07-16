using QPP.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Runtime
{
    public interface IRunnable
    {
        void Run(DataArgs args);
    }
}
