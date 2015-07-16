using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Data
{
    public interface ITransaction : IDisposable
    {
        /// <summary>
        /// 認可資料庫交易。
        /// </summary>
        void Commit();
        /// <summary>
        /// 從暫止狀態復原交易。
        /// </summary>
        void Rollback();
    }
}
