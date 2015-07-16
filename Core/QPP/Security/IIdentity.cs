using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Security
{
    /// <summary>
    /// 身份标识
    /// </summary>
    public interface IIdentity
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        string Id { get; }
    }
}
