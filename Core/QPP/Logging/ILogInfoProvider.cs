using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Logging
{
    public interface ILogInfoProvider
    {
        string GetIP();
        string GetReporter();
        string GetClientInfo();
    }
}
