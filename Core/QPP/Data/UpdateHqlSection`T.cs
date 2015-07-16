using QPP.Filtering;
using QPP.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace QPP.Data
{
    public class UpdateHqlSection<T>
    {
        Criteria<T> m_Where = new Criteria<T>();
        Dictionary<string, object> m_Values = new Dictionary<string, object>();
        T m_Model;
        ITransaction m_Tran;
        IDataAccess m_DataAccess;

        public UpdateHqlSection(IDataAccess dataAccess)
        {
            m_DataAccess = dataAccess;
        }
        public UpdateHqlSection(IDataAccess dataAccess, ITransaction tran)
        {
            m_Tran = tran;
            m_DataAccess = dataAccess;
        }
        public UpdateHqlSection(T model, IDataAccess dataAccess)
        {
            m_DataAccess = dataAccess;
            m_Model = model;
        }
        public UpdateHqlSection(T model, IDataAccess dataAccess, ITransaction tran)
        {
            m_Tran = tran;
            m_DataAccess = dataAccess;
            m_Model = model;
        }

        public UpdateHqlSection<T> Where(Expression<Func<T, bool>> expr)
        {
            m_Where.And(expr);
            return this;
        }
        public UpdateHqlSection<T> AddColumn<V>(Expression<Func<T, V>> expr)
        {
            var property = (expr.Body as MemberExpression).Member.Name;
            var value = expr.Compile().Invoke(m_Model);
            m_Values.Add(property, value);
            return this;
        }
        public UpdateHqlSection<T> Set<V>(Expression<Func<T, V>> expr, object value)
        {
            var property = (expr.Body as MemberExpression).Member.Name;
            m_Values.Add(property, value);
            return this;
        }
        public UpdateHqlSection<T> Set(string property, object value)
        {
            m_Values.Add(property, value);
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
            hql.Hql = "update " + typeof(T).Name + " set ";
            int index = 0;
            foreach (var v in m_Values)
            {
                hql.Hql += "{0}=:p{1},".FormatArgs(v.Key, index);
                hql.AddParameter("p" + index, v.Value);
                index++;
            }
            hql.Hql = hql.Hql.TrimEnd(',');
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
