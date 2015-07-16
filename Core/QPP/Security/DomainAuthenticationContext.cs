using QPP.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Security
{
    public class DomainAuthenticationContext : IAuthenticationContext
    {
        public DomainAuthenticationContext()
        {
            AuthDataName = "AuthTicket";
            AuthenticationMode = Security.AuthenticationMode.Ticket;
        }

        public string GetTicket()
        {
            return AppDomain.CurrentDomain.GetData(AuthDataName) as string;
        }

        public void SetTicket(string ticket)
        {
            AppDomain.CurrentDomain.SetData(AuthDataName, ticket);
        }

        public AuthenticationMode AuthenticationMode
        {
            get;
            set;
        }

        public string AuthDataName { get; set; }
    }
}
