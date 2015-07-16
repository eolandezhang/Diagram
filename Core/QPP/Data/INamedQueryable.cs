using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Data
{
    public interface INamedQueryable
    {
        /// <summary>
        /// 执行命名查询，返回单个值
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        object ExecuteScalar(ITransaction tran, NamedQuery query);
        /// <summary>
        /// 执行命名查询，返回受影响行数
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        int ExecuteNonQuery(ITransaction tran, NamedQuery query);
        /// <summary>
        /// 根据命名查询得到列表
        /// </summary>
        /// <param name="query">命名查询</param>
        /// <returns>列表</returns>
        IList<T> GetList<T>(ITransaction tran, NamedQuery query);
    }
}
