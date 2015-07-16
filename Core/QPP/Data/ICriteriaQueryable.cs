using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QPP.Filtering;

namespace QPP.Data
{
    public interface ICriteriaQueryable
    {
        /// <summary>
        /// 根据查询标准到得记录数量
        /// </summary>
        /// <param name="criteria">查询标准</param>
        /// <returns>记录数量</returns>
        int Count<T>(ITransaction tran, CriteriaOperator criteria) where T : class;
        ///// <summary>
        ///// 指定字段，根据条件查询得到列表
        ///// </summary>
        ///// <param name="columns"></param>
        ///// <param name="condition"></param>
        ///// <param name="sort"></param>
        ///// <param name="skipRows"></param>
        ///// <param name="maxRows"></param>
        ///// <param name="distinct"></param>
        ///// <returns></returns>
        //IList<object> GetList<T>(ITransaction tran, CriteriaOperator[] columns, CriteriaOperator condition, string sort = "",
        //    int skipRows = 0, int maxRows = -1, bool distinct = false) where T : class;
        /// <summary>
        /// 根据查询标准得到一条记录
        /// </summary>
        /// <param name="criteria">查询标准</param>
        /// <returns>一条记录</returns>
        T Get<T>(ITransaction tran, CriteriaOperator criteria) where T : class;
        ///// <summary>
        ///// 指定字段，根据条件查询得到对象
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="tran"></param>
        ///// <param name="columns"></param>
        ///// <param name="condition"></param>
        ///// <returns></returns>
        //object Get<T>(ITransaction tran, CriteriaOperator[] columns, CriteriaOperator condition) where T : class;
        /// <summary>
        /// 根据查询标准到得列表
        /// </summary>
        /// <param name="criteria">查询标准</param>
        /// <param name="sort">排序，eg:"Name ASC,UpdateDate DESC"</param>
        /// <param name="skipRows">跳过行数，默认为0</param>
        /// <param name="maxRows">最大行数，默认为-1</param>
        /// <returns>列表</returns>
        IList<T> GetList<T>(ITransaction tran, CriteriaOperator criteria, string sort = "", int skipRows = 0, int maxRows = -1) where T : class;
    }
}
