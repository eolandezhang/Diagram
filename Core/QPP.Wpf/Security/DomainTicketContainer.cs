using QPP.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Security
{
    public class DomainTicketContainer : ITicketContainer
    {
        public DomainTicketContainer()
        {
            AuthDataName = "AuthTicket";
            Mode = AuthenticationMode.Ticket;
        }

        public string GetTicket()
        {
            return AppDomain.CurrentDomain.GetData(AuthDataName) as string;
        }

        public void SetTicket(string ticket)
        {
            AppDomain.CurrentDomain.SetData(AuthDataName, ticket);
        }

        public AuthenticationMode Mode
        {
            get;
            set;
        }

        public string AuthDataName { get; set; }
    }
}
