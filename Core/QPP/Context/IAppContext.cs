using QPP.Collections;
using QPP.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Context
{
    /// <summary>
    /// 應用上下文
    /// </summary>
    public interface IAppContext
    {
        /// <summary>
        /// 應用類名
        /// </summary>
        string AppTypeName { get; }
        
        /// <summary>
        /// 用戶信息
        /// </summary>
        UserIdentity User { get; set; }

        /// <summary>
        /// 上下文擴展內容
        /// </summary>
        NameObjectCollection Content { get; }

        /// <summary>
        /// 父親上下文
        /// </summary>
        IAppContext Parent { get; }
    }
}
