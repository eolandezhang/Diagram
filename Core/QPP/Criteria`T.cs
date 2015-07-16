using QPP.Filtering;
using QPP.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace QPP
{
    /// <summary>
    /// 標準查詢構造器，使用Lambda表達式構建CriteriaQuery
    /// </summary>
    public class Criteria
    {
        string m_sort;
        string m_defaultSort;
        string m_Criteria;
        int m_MaxRows = -1;

        /// <summary>
        /// 跳過的行數，用於分頁
        /// </summary>
        public int SkipRows { get; private set; }
        /// <summary>
        /// 最大的行數，用於分頁
        /// </summary>
        public int MaxRows
        {
            get { return m_MaxRows; }
            private set { m_MaxRows = value; }
        }
        /// <summary>
        /// 排序，優先按SetSort方法設定的排序，如果沒設置，則按SetDefaultSort方法設定的排序
        /// </summary>
        public string Sort
        {
            get { return m_sort.IsNullOrEmpty() ? m_defaultSort : m_sort; }
            private set { m_sort = value; }
        }
        /// <summary>
        /// 設置MaxRows
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public Criteria Take(int rows)
        {
            MaxRows = rows;
            return this;
        }
        /// <summary>
        /// 設置SkipRows
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public Criteria Skip(int rows)
        {
            SkipRows = rows;
            return this;
        }
        /// <summary>
        /// 設定排序
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        public Criteria SetSort(string sort)
        {
            if (sort.IsNotEmpty())
            {
                if (sort.EndsWith(" DESC", StringComparison.OrdinalIgnoreCase) || sort.EndsWith(" ASC", StringComparison.OrdinalIgnoreCase))
                    Sort = sort;
                else
                    Sort = sort + " ASC";
            }
            return this;
        }
        /// <summary>
        /// 設定默認排序
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        public Criteria SetDefaultSort(string sort)
        {
            if (sort.IsNotEmpty())
            {
                if (sort.EndsWith(" DESC", StringComparison.OrdinalIgnoreCase) || sort.EndsWith(" ASC", StringComparison.OrdinalIgnoreCase))
                    m_defaultSort = sort;
                else
                    m_defaultSort = sort + " ASC";
            }
            return this;
        }
        /// <summary>
        /// 增加ASC排序
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public Criteria ASC(string column)
        {
            if (Sort.IsNotEmpty())
                Sort += ",";
            Sort += column + " ASC";
            return this;
        }
        /// <summary>
        /// 增加DESC排序
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public Criteria DESC(string column)
        {
            if (Sort.IsNotEmpty())
                Sort += ",";
            Sort += column + " DESC";
            return this;
        }

        /// <summary>
        /// 創建Criteria。
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static Criteria Create(string criteria = null)
        {
            return new Criteria { m_Criteria = criteria };
        }
        /// <summary>
        /// 創建Criteria&lt;T&gt;。
        /// <example>例如Criteria.Criteria&lt;UserDomain&gt;(u =&gt; u.Name=="name");</example>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Criteria<T> Create<T>(Expression<Func<T, bool>> expr)
        {
            return new Criteria<T>().And(expr);
        }
        /// <summary>
        /// 創建Criteria&lt;T&gt;。
        /// <example>例如Criteria.Criteria&lt;UserDomain&gt;("[Name]=='name'");</example>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static Criteria<T> Create<T>(string criteria = null)
        {
            return new Criteria<T>().And(criteria);
        }
        /// <summary>
        /// 創建Criteria&lt;T&gt;。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Criteria<T> Create<T>(CriteriaQuery query)
        {
            var criteria = new Criteria<T>();
            if (query.Criteria.IsNotEmpty())
                criteria.Value = CriteriaOperator.TryParse(query.Criteria);
            criteria.MaxRows = query.MaxRows;
            criteria.SkipRows = query.SkipRows;
            criteria.Sort = query.Sort;
            return criteria;
        }

        /// <summary>
        /// 轉為CriteriaQuery
        /// </summary>
        /// <returns></returns>
        public virtual CriteriaQuery ToQuery()
        {
            var query = new CriteriaQuery();
            query.Criteria = m_Criteria;
            query.MaxRows = MaxRows;
            query.SkipRows = SkipRows;
            query.Sort = Sort;
            return query;
        }
    }
    /// <summary>
    /// 強類型標準查詢構造器，使用Lambda表達式構建CriteriaQuery
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Criteria<T> : Criteria
    {
        CriteriaOperator m_Criteria = CriteriaOperator.Empty;

        /// <summary>
        /// 標準查詢的值
        /// </summary>
        public CriteriaOperator Value
        {
            get { return m_Criteria; }
            set { m_Criteria = value; }
        }
        /// <summary>
        /// 設置MaxRows
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public new Criteria<T> Take(int rows)
        {
            base.Take(rows);
            return this;
        }
        /// <summary>
        /// 設置SkipRows
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public new Criteria<T> Skip(int rows)
        {
            base.Skip(rows);
            return this;
        }
        /// <summary>
        /// 設定排序
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        public new Criteria<T> SetSort(string sort)
        {
            base.SetSort(sort);
            return this;
        }
        /// <summary>
        /// 設定默認排序
        /// <example>例如Criteria.Criteria&lt;UserDomain&gt;().SetDefaultSort("UpdateDate DESC");</example>
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        public new Criteria<T> SetDefaultSort(string sort)
        {
            base.SetDefaultSort(sort);
            return this;
        }
        /// <summary>
        /// 設定默認排序
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        public Criteria<T> SetDefaultSort(Order<T> sort)
        {
            base.SetDefaultSort(sort.Value);
            return this;
        }
        /// <summary>
        /// 增加ASC排序
        /// <example>例如Criteria.Criteria&lt;UserDomain&gt;().ASC("UpdateDate");</example>
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public new Criteria<T> ASC(string column)
        {
            base.ASC(column);
            return this;
        }
        /// <summary>
        /// 增加DESC排序
        /// <example>例如Criteria.Criteria&lt;UserDomain&gt;().DESC("UpdateDate");</example>
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public new Criteria<T> DESC(string column)
        {
            base.DESC(column);
            return this;
        }
        /// <summary>
        /// 增加ASC排序
        /// <example>例如Criteria.Criteria&lt;UserDomain&gt;().ASC(p =&gt; p.UpdateDate);</example>
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public Criteria<T> ASC<V>(Expression<Func<T, V>> expr)
        {
            var column = (expr.Body as MemberExpression).Member.Name;
            return ASC(column);
        }
        /// <summary>
        /// 增加DESC排序
        /// <example>例如Criteria.Criteria&lt;UserDomain&gt;().DESC(p =&gt; p.UpdateDate);</example>
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        public Criteria<T> DESC<V>(Expression<Func<T, V>> expr)
        {
            var column = (expr.Body as MemberExpression).Member.Name;
            return DESC(column);
        }
        /// <summary>
        /// 增加And條件
        /// <example>例如Criteria.Criteria&lt;UserDomain&gt;().And(p =&gt; p.Name == "name");</example>
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public Criteria<T> And(Expression<Func<T, bool>> expr)
        {
            m_Criteria &= ExpressionProcessor.ProcessLambdaExpression(expr);
            return this;
        }

        /// <summary>
        /// 增加Exists條件。
        /// <example>例如Criteria.Criteria&lt;UserDomain&gt;().Exists&lt;UserInRoleDomain&gt;((u, r) =&gt; u.Id == r.UserId &amp;&amp; r.RoleId == "admin");</example>
        /// </summary>
        /// <typeparam name="V">聚合類型</typeparam>
        /// <param name="expr">條件表達式</param>
        /// <returns></returns>
        public Criteria<T> Exists<V>(Expression<Func<T, V, bool>> expr)
        {
            AggregateOperand operand = new AggregateOperand();
            operand.AggregateType = Aggregate.Exists;
            operand.CollectionProperty = new OperandProperty(typeof(V).FullName + "," + expr.Parameters[1].Name);
            operand.Condition = ExpressionProcessor.ProcessLambdaExpression(expr, true);
            m_Criteria &= operand;
            return this;
        }

        /// <summary>
        /// 增加Exists條件。
        /// </summary>
        /// <typeparam name="V">聚合類型</typeparam>
        /// <param name="alias">聚合類型的別名</param>
        /// <param name="condition">聚合條件</param>
        /// <returns></returns>
        public Criteria<T> Exists<V>(string alias, CriteriaOperator condition)
        {
            AggregateOperand operand = new AggregateOperand();
            operand.AggregateType = Aggregate.Exists;
            operand.CollectionProperty = new OperandProperty(typeof(V).FullName + "," + alias);
            operand.Condition = condition;
            m_Criteria &= operand;
            return this;
        }

        /// <summary>
        /// 增加Exists條件。
        /// </summary>
        /// <typeparam name="V">聚合類型</typeparam>
        /// <param name="alias">聚合類型的別名</param>
        /// <param name="condition">聚合條件</param>
        /// <returns></returns>
        public Criteria<T> Exists<V>(string alias, string condition)
        {
            return Exists<V>(alias, CriteriaOperator.Parse(condition));
        }

        /// <summary>
        /// 增加Or條件
        /// <example>例如Criteria.Criteria&lt;UserDomain&gt;().Or(p =&gt; p.Name == "name");</example>
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public Criteria<T> Or(Expression<Func<T, bool>> expr)
        {
            m_Criteria |= ExpressionProcessor.ProcessLambdaExpression(expr);
            return this;
        }
        /// <summary>
        /// 增加And條件
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public Criteria<T> And(string criteria)
        {
            if (criteria.IsNotEmpty())
                m_Criteria &= CriteriaOperator.TryParse(criteria);
            return this;
        }
        /// <summary>
        /// 增加Or條件
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public Criteria<T> Or(string criteria)
        {
            if (criteria.IsNotEmpty())
                m_Criteria |= CriteriaOperator.TryParse(criteria);
            return this;
        }
        /// <summary>
        /// 增加And條件
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public Criteria<T> And(CriteriaOperator op)
        {
            m_Criteria &= op;
            return this;
        }
        /// <summary>
        /// 增加Or條件
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public Criteria<T> Or(CriteriaOperator op)
        {
            m_Criteria |= op;
            return this;
        }
        /// <summary>
        /// 轉為CriteriaQuery
        /// </summary>
        /// <returns></returns>
        public new CriteriaQuery ToQuery()
        {
            var query = new CriteriaQuery();
            if (object.Equals(m_Criteria, CriteriaOperator.Empty))
                query.Criteria = "";
            else
                query.Criteria = m_Criteria.ToString();
            query.MaxRows = MaxRows;
            query.SkipRows = SkipRows;
            query.Sort = Sort;
            return query;
        }
    }
}
