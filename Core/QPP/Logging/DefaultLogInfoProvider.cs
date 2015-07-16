using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace QPP.Logging
{
    public class DefaultLogInfoProvider : ILogInfoProvider
    {
        public virtual string GetIP()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry host = Dns.GetHostEntry(hostName);
            return host.AddressList[0].ToString();
        }

        public virtual string GetReporter()
        {
            return Environment.UserName;
        }

        public virtual string GetClientInfo()
        {
            return "OSVersion:{0}\r\nCLR Version:{1}"
                .FormatArgs(Environment.OSVersion, Environment.Version);
        }
    }
}
