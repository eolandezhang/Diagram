using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Security
{
    public class DomainTicketContainer : ITicketContainer
    {
        public AuthenticationMode Mode
        {
            get { throw new NotImplementedException(); }
        }

        public string GetTicket()
        {
            throw new NotImplementedException();
        }

        public void SetTicket(string ticket)
        {
            throw new NotImplementedException();
        }
    }
}
