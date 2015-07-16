using QPP.Filtering;
using QPP.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace QPP.Data
{
    public class DeleteHqlSection<T>
    {
        Criteria<T> m_Where = new Criteria<T>();
        ITransaction m_Tran;
        IDataAccess m_DataAccess;
        
        public DeleteHqlSection(IDataAccess dataAccess)
        {
            m_DataAccess = dataAccess;
        }
        public DeleteHqlSection(IDataAccess dataAccess, ITransaction tran)
        {
            m_Tran = tran;
            m_DataAccess = dataAccess;
        }

        public DeleteHqlSection<T> Where(Expression<Func<T, bool>> expr)
        {
            m_Where.And(expr);
            return this;
        }

        public int Excute()
        {
            var query = ToQuery();
            return m_DataAccess.ExecuteNonQuery(m_Tran, query);
        }

        public HqlQuery ToQuery()
        {
            var hql = new HqlQuery();
            hql.Hql = "delete from " + typeof(T).Name;
            Dictionary<string, object> parameter;
            var condition = CriteriaToHqlWithParametersProcessor.ToString(m_Where.Value, out parameter);
            if (condition.IsNotEmpty())
            {
                hql.Hql += " where " + condition;
                foreach (var p in parameter)
                    hql.AddParameter(p.Key, p.Value);
            }
            return hql;
        }
    }
}
