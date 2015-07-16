using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Security
{
    public interface ITicketContainer
    {
        AuthenticationMode Mode { get; }
        string GetTicket();
        void SetTicket(string ticket);
    }
}
