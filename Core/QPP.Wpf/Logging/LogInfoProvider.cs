using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QPP.Logging;

namespace QPP.Wpf.Logging
{
    public class LogInfoProvider : DefaultLogInfoProvider
    {
        public override string GetReporter()
        {
            //var user = RuntimeContext.GetOriginalAppContext().User;
            //if (user == null || user.UserName.IsNullOrEmpty())
                return base.GetReporter();
            //return user.UserName;
        }
    }
}
