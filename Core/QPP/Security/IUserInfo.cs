using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Security
{
    /// <summary>
    /// 用戶信息
    /// </summary>
    public interface IUserInfo
    {
        /// <summary>
        /// 用戶名
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        string UserName { get; set; }
        /// <summary>
        /// 郵箱
        /// </summary>
        string Email { get; set; }
        /// <summary>
        /// 是否在職
        /// </summary>
        bool Inservice { get; set; }
    }
}
