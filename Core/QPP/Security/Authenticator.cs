using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Security
{
    public class Authenticator : IAuthenticatable
    {
        public IPrincipal Principal { get; set; }
        /// <summary>
        /// Check if the command is permitted.
        /// </summary>
        /// <exception cref="NotPermittedException"></exception>
        /// <param name="command"></param>
        protected virtual void CheckPermission(string command)
        {
            if (Principal == null || !Principal.IsPermitted(command))
                throw new NotPermittedException(command);
        }
    }
}
