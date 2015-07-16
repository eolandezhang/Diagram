using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Security
{
    /// <summary>
    /// 当事人
    /// </summary>
    public interface IPrincipal
    {
        /// <summary>
        /// 是否允许指令
        /// </summary>
        /// <param name="command">指令名称</param>
        /// <returns>true:允许，false:不允许</returns>
        bool IsPermitted(string command);
        /// <summary>
        /// 身份标示
        /// </summary>
        IIdentity Identity { get; }
    }
}
