using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Security
{
    public interface IAuthenticatable
    {
        /// <summary>
        /// 当事人
        /// </summary>
        IPrincipal Principal { get; set; }
    }
}
