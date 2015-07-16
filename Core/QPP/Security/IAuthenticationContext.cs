using QPP.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Security
{
    public interface IAuthenticationContext
    {
        AuthenticationMode AuthenticationMode { get; }
        string GetTicket();
        void SetTicket(string ticket);
    }
}
