using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QPP.Filtering;
using QPP.ComponentModel;

namespace QPP.Data
{
    public interface IDataAccess : ITransactional, ICriteriaQueryable, INamedQueryable, IHqlQueryable
    {
        /// <summary>
        /// 插入一条记录
        /// </summary>
        /// <param name="model">要插入的记录</param>
        /// <returns></returns>
        object Add(ITransaction tran, object model);
        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="model">要更新的记录</param>
        /// <returns></returns>
        void Update(ITransaction tran, object model);
        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="model">要删除的记录</param>
        /// <returns></returns>
        void Delete(ITransaction tran, object model);
    }
}
