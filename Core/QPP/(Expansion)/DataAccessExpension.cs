using QPP.ComponentModel.DataAnnotations;
using QPP.Data;
using QPP.Filtering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace QPP
{
    public static class DataAccessExpension
    {
        /// <summary>
        /// <para>update property value from domain</para>
        /// <para>ctx.HqlUpdate&lt;UserDomain&gt;(domain).AddColumn(p =&gt; p.Status).Where(p =&gt; p.Id = id);</para>
        /// <para>update property value directly</para>
        /// <para>ctx.HqlUpdate&lt;UserDomain&gt;().Set(p =&gt; p.Status, 0).Where(p =&gt; p.Id = id);</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctx"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static UpdateHqlSection<T> HqlUpdate<T>(this TransactionContext ctx, T domain = null) where T : class
        {
            return new UpdateHqlSection<T>(domain, ctx.DataAccess, ctx.Transaction);
        }

        public static DeleteHqlSection<T> HqlDelete<T>(this TransactionContext ctx)
        {
            return new DeleteHqlSection<T>(ctx.DataAccess, ctx.Transaction);
        }

        public static void DeleteOrMarkDelete<T>(this TransactionContext ctx, T model)
        {
            var t = typeof(T);
            foreach (var p in t.GetProperties())
            {
                var attr = p.GetCustomAttributes(typeof(DeleteMarkAttribute), true).FirstOrDefault();
                if (attr != null)
                {
                    HqlQuery hql = new HqlQuery();
                    var pDeleteBy = t.GetProperties().FirstOrDefault(d => d.GetCustomAttributes(typeof(DeleteByAttribute), true).Any());
                    var pDeleteDate = t.GetProperties().FirstOrDefault(d => d.GetCustomAttributes(typeof(DeleteDateAttribute), true).Any());
                    string deleteInfo = "";
                    if (pDeleteBy != null)
                    {
                        deleteInfo += "," + pDeleteBy.Name + "=:" + pDeleteBy.Name;
                        hql.AddParameter(pDeleteBy.Name, pDeleteBy.GetValue(model, null));
                    }
                    if (pDeleteDate != null)
                    {
                        deleteInfo += "," + pDeleteDate.Name + "=:" + pDeleteDate.Name;
                        hql.AddParameter(pDeleteDate.Name, pDeleteDate.GetValue(model, null));
                    }
                    string where = "";
                    var keys = t.GetProperties().Where(d => d.GetCustomAttributes(typeof(KeyAttribute), true).Any());
                    foreach (var k in keys)
                    {
                        if (where.IsNotEmpty())
                            where += " and ";
                        where += k.Name + "=:" + k.Name;
                        hql.AddParameter(k.Name, k.GetValue(model, null));
                    }
                    hql.Hql = "update {0} set {1}=:deletemark{2} where {3}".FormatArgs(t.Name, p.Name, deleteInfo, where);
                    hql.AddParameter("deletemark", ((DeleteMarkAttribute)attr).Mark);
                    ctx.ExecuteNonQuery(hql);
                    return;
                }
            }
            ctx.Delete(model);
        }

        public static void DeleteOrMarkDelete<T>(this IDataAccess da, T model)
        {
            var t = typeof(T);
            foreach (var p in t.GetProperties())
            {
                var attr = p.GetCustomAttributes(typeof(DeleteMarkAttribute), true).FirstOrDefault();
                if (attr != null)
                {
                    HqlQuery hql = new HqlQuery();
                    var pDeleteBy = t.GetProperties().FirstOrDefault(d => d.GetCustomAttributes(typeof(DeleteByAttribute), true).Any());
                    var pDeleteDate = t.GetProperties().FirstOrDefault(d => d.GetCustomAttributes(typeof(DeleteDateAttribute), true).Any());
                    string deleteInfo = "";
                    if (pDeleteBy != null)
                    {
                        deleteInfo += "," + pDeleteBy.Name + "=:" + pDeleteBy.Name;
                        hql.AddParameter(pDeleteBy.Name, pDeleteBy.GetValue(model, null));
                    }
                    if (pDeleteDate != null)
                    {
                        deleteInfo += "," + pDeleteDate.Name + "=:" + pDeleteDate.Name;
                        hql.AddParameter(pDeleteDate.Name, pDeleteDate.GetValue(model, null));
                    }
                    string where = "";
                    var keys = t.GetProperties().Where(d => d.GetCustomAttributes(typeof(KeyAttribute), true).Any());
                    foreach (var k in keys)
                    {
                        if (where.IsNotEmpty())
                            where += " and ";
                        where += k.Name + "=:" + k.Name;
                        hql.AddParameter(k.Name, k.GetValue(model, null));
                    }
                    hql.Hql = "update {0} set {1}=:deletemark{2} where {3}".FormatArgs(t.Name, p.Name, deleteInfo, where);
                    hql.AddParameter("deletemark", ((DeleteMarkAttribute)attr).Mark);
                    da.ExecuteNonQuery(hql);
                    return;
                }
            }
            da.Delete(model);
        }

        /// <summary>
        /// <para>update property value from domain</para>
        /// <para>da.HqlUpdate&ltUserDomain&gt(domain).AddColumn(p =&gt p.Status).Where(p =&gt p.Id = id);</para>
        /// <para>update property value directly</para>
        /// <para>da.HqlUpdate&ltUserDomain&gt().Set(p =&gt p.Status, 0).Where(p =&gt p.Id = id);</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="da"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static UpdateHqlSection<T> HqlUpdate<T>(this IDataAccess da, T domain = null) where T : class
        {
            return new UpdateHqlSection<T>(domain, da);
        }

        public static DeleteHqlSection<T> HqlDelete<T>(this IDataAccess da)
        {
            return new DeleteHqlSection<T>(da);
        }

        public static CriteriaOperator Filter<T>(this IDataAccess da, Expression<Func<T, bool>> expr)
        {
            return Criteria.Create<T>().And(expr).Value;
        }

        public static TransactionContext GetTransactionContext(this IDataAccess da)
        {
            return new TransactionContext(da);
        }

        public static TransactionContext GetTransactionContext(this IDataAccess da, IsolationLevel i)
        {
            return new TransactionContext(da, i);
        }

        public static T Get<T>(this IDataAccess da, ITransaction tran, Expression<Func<T, bool>> expr) where T : class
        {
            return da.Get<T>(tran, da.Filter<T>(expr));
        }

        public static IList<T> GetList<T>(this IDataAccess da, ITransaction tran, Criteria<T> query) where T : class
        {
            return da.GetList<T>(tran, query.Value, query.Sort, query.SkipRows, query.MaxRows);
        }

        public static int Count<T>(this IDataAccess da, ITransaction tran, Criteria<T> query) where T : class
        {
            return da.Count<T>(tran, query.Value);
        }

        public static T Get<T>(this IDataAccess da, Expression<Func<T, bool>> expr) where T : class
        {
            return da.Get<T>(null, da.Filter<T>(expr));
        }

        public static IList<T> GetList<T>(this IDataAccess da, Expression<Func<T, bool>> expr) where T : class
        {
            return da.GetList<T>(null, da.Filter<T>(expr));
        }

        public static IList<T> GetList<T>(this IDataAccess da, Criteria<T> query) where T : class
        {
            return da.GetList<T>(null, query.Value, query.Sort, query.SkipRows, query.MaxRows);
        }

        public static int Count<T>(this IDataAccess da, Criteria<T> query) where T : class
        {
            return da.Count<T>(null, query.Value);
        }

        public static object Add(this IDataAccess da, object model)
        {
            return da.Add(null, model);
        }

        public static void Update(this IDataAccess da, object model)
        {
            da.Update(null, model);
        }

        public static void Delete(this IDataAccess da, object model)
        {
            da.Delete(null, model);
        }

        public static int Count<T>(this IDataAccess da, CriteriaOperator criteria) where T : class
        {
            return da.Count<T>(null, criteria);
        }

        //public static IList<object> GetList<T>(this IDataAccess da, CriteriaOperator[] columns, Filtering.CriteriaOperator condition, string sort = "", int skipRows = 0, int maxRows = -1, bool distinct = false) where T : class
        //{
        //    return da.GetList<T>(null, columns, condition, sort, skipRows, maxRows, distinct);
        //}

        public static T Get<T>(this IDataAccess da, CriteriaOperator criteria) where T : class
        {
            return da.Get<T>(null, criteria);
        }

        //public static object Get<T>(this IDataAccess da, CriteriaOperator[] columns, Filtering.CriteriaOperator condition) where T : class
        //{
        //    return da.Get<T>(null, columns, condition);
        //}

        public static IList<T> GetList<T>(this IDataAccess da, CriteriaOperator criteria, string sort = "", int skipRows = 0, int maxRows = -1) where T : class
        {
            return da.GetList<T>(null, criteria, sort, skipRows, maxRows);
        }

        public static object ExecuteScalar(this IDataAccess da, NamedQuery query)
        {
            return da.ExecuteScalar(null, query);
        }

        public static int ExecuteNonQuery(this IDataAccess da, NamedQuery query)
        {
            return da.ExecuteNonQuery(null, query);
        }

        public static IList<T> GetList<T>(this IDataAccess da, NamedQuery query)
        {
            return da.GetList<T>(null, query);
        }

        public static object ExecuteScalar(this IDataAccess da, HqlQuery hqlQuery)
        {
            return da.ExecuteScalar(null, hqlQuery);
        }

        public static int ExecuteNonQuery(this IDataAccess da, HqlQuery hqlQuery)
        {
            return da.ExecuteNonQuery(null, hqlQuery);
        }

        public static IList<T> GetList<T>(this IDataAccess da, HqlQuery hqlQuery)
        {
            return da.GetList<T>(null, hqlQuery);
        }
    }
}
