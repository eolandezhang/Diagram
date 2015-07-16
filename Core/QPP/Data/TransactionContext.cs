using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QPP.Filtering;
using System.Linq.Expressions;

namespace QPP.Data
{
    public class TransactionContext : IDisposable
    {
        bool isCommitted;

        public IDataAccess DataAccess { get; private set; }
        public ITransaction Transaction { get; private set; }

        public void Commit()
        {
            Transaction.Commit();
            isCommitted = true;
        }

        public TransactionContext(IDataAccess da)
        {
            DataAccess = da;
            Transaction = da.BeginTransaction();
        }

        public TransactionContext(IDataAccess da, IsolationLevel i)
        {
            DataAccess = da;
            Transaction = da.BeginTransaction(i);
        }

        public object Add(object model)
        {
            return DataAccess.Add(Transaction, model);
        }

        public void Update(object model)
        {
            DataAccess.Update(Transaction, model);
        }

        public void Delete(object model)
        {
            DataAccess.Delete(Transaction, model);
        }

        public int Count<T>(CriteriaOperator criteria) where T : class
        {
            return DataAccess.Count<T>(Transaction, criteria);
        }

        //public IList<object> GetList<T>(CriteriaOperator[] columns, Filtering.CriteriaOperator condition, string sort = "", int skipRows = 0, int maxRows = -1, bool distinct = false) where T : class
        //{
        //    return DataAccess.GetList<T>(Transaction, columns, condition, sort, skipRows, maxRows, distinct);
        //}

        public T Get<T>(CriteriaOperator criteria) where T : class
        {
            return DataAccess.Get<T>(Transaction, criteria);
        }

        //public object Get<T>(CriteriaOperator[] columns, Filtering.CriteriaOperator condition) where T : class
        //{
        //    return DataAccess.Get<T>(Transaction, columns, condition);
        //}

        public IList<T> GetList<T>(CriteriaOperator criteria, string sort = "", int skipRows = 0, int maxRows = -1) where T : class
        {
            return DataAccess.GetList<T>(Transaction, criteria, sort, skipRows, maxRows);
        }

        public object ExecuteScalar(NamedQuery query)
        {
            return DataAccess.ExecuteScalar(Transaction, query);
        }

        public int ExecuteNonQuery(NamedQuery query)
        {
            return DataAccess.ExecuteNonQuery(Transaction, query);
        }

        public IList<T> GetList<T>(NamedQuery query)
        {
            return DataAccess.GetList<T>(Transaction, query);
        }

        public object ExecuteScalar(HqlQuery hqlQuery)
        {
            return DataAccess.ExecuteScalar(Transaction, hqlQuery);
        }

        public int ExecuteNonQuery(HqlQuery hqlQuery)
        {
            return DataAccess.ExecuteNonQuery(Transaction, hqlQuery);
        }

        public IList<T> GetList<T>(HqlQuery hqlQuery)
        {
            return DataAccess.GetList<T>(Transaction, hqlQuery);
        }

        public T Get<T>(Expression<Func<T, bool>> expr) where T : class
        {
            return DataAccess.Get<T>(Transaction, DataAccess.Filter<T>(expr));
        }

        public IList<T> GetList<T>(Criteria<T> query) where T : class
        {
            return DataAccess.GetList<T>(Transaction, query.Value, query.Sort, query.SkipRows, query.MaxRows);
        }

        public int Count<T>(Criteria<T> query) where T : class
        {
            return DataAccess.Count<T>(Transaction, query.Value);
        }

        public void Dispose()
        {
            if (!isCommitted)
                Transaction.Rollback();
        }
    }
}
