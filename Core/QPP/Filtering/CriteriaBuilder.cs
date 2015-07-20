using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using QPP.Utils;

namespace QPP.Filtering
{
    public class CriteriaBuilder<T>
    {
        CriteriaOperator value = CriteriaOperator.Empty;

        public CriteriaOperator Value
        {
            get { return value; }
        }

        public CriteriaBuilder()
        {
        }

        public CriteriaBuilder(Expression<Func<T, bool>> expr)
        {
            And(expr);
        }

        public static CriteriaBuilder<T> Create(Expression<Func<T, bool>> expr)
        {
            return new CriteriaBuilder<T>().And(expr);
        }

        public static CriteriaBuilder<T> Create(CriteriaOperator criteria)
        {
            return new CriteriaBuilder<T>().And(criteria);
        }

        protected virtual CriteriaBuilder<T> Appand(CriteriaOperator op, GroupOperatorType type)
        {
            if (object.ReferenceEquals(value, CriteriaOperator.Empty))
                value = op;
            else if (type == GroupOperatorType.And)
                value = CriteriaOperator.And(value, op);
            else if (type == GroupOperatorType.Or)
                value = CriteriaOperator.Or(value, op);
            return this;
        }

        public CriteriaBuilder<T> And(Expression<Func<T, bool>> expr)
        {
            Appand(ExpressionProcessor.ProcessLambdaExpression(expr), GroupOperatorType.And);
            return this;
        }

        public CriteriaBuilder<T> And(CriteriaOperator criteria)
        {
            Appand(criteria, GroupOperatorType.And);
            return this;
        }

        public CriteriaBuilder<T> Or(Expression<Func<T, bool>> expr)
        {
            Appand(ExpressionProcessor.ProcessLambdaExpression(expr), GroupOperatorType.Or);
            return this;
        }

        public CriteriaBuilder<T> Or(CriteriaOperator criteria)
        {
            Appand(criteria, GroupOperatorType.Or);
            return this;
        }
    }
}
