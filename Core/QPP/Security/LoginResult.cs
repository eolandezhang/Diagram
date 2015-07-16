using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Security
{
    public enum LoginResult
    {
        /// <summary>
        /// 登陸成功
        /// </summary>
        Pass,
        /// <summary>
        /// 用戶名不存在
        /// </summary>
        UserNoFound,
        /// <summary>
        /// 密碼不正確
        /// </summary>
        PwdInvalid,
        /// <summary>
        /// 用戶被鎖定
        /// </summary>
        Locked,
        /// <summary>
        /// 秘密過期
        /// </summary>
        PwdOverdue
    }
}
