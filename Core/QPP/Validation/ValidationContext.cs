using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace QPP.Validation
{
    public class ValidationContext<TEntity, TResult>
    {
        TEntity m_Entity;
        Validator m_Validator;
        Expression<Func<TEntity, TResult>> m_Property;
        int m_RowNum;

        public ValidationContext(Validator validator, TEntity entity,
            Expression<Func<TEntity, TResult>> expr, int rowNum)
        {
            m_Entity = entity;
            m_Validator = validator;
            m_Property = expr;
            m_RowNum = rowNum;
        }

        string GetPropertyName()
        {
            MemberExpression expr = m_Property.Body as MemberExpression;
            return expr != null ? typeof(TEntity).Name + "." + expr.Member.Name : "";
        }

        public ValidationContext<TEntity, TResult> IsRequired()
        {
            m_Validator.Require(m_Property.Compile().Invoke(m_Entity), GetPropertyName(), m_RowNum);
            return this;
        }

        public ValidationContext<TEntity, TResult> Assert(bool condition, ErrorText error)
        {
            m_Validator.Assert(condition, GetPropertyName(), error, m_RowNum);
            return this;
        }

        public ValidationContext<TEntity, TResult> MaxLength(int length)
        {
            m_Validator.MaxLength(m_Property.Compile().Invoke(m_Entity).ToSafeString(), length, GetPropertyName(), m_RowNum);
            return this;
        }

        public ValidationContext<TEntity, TResult> MinLength(int length)
        {
            m_Validator.MinLength(m_Property.Compile().Invoke(m_Entity).ToSafeString(), length, GetPropertyName(), m_RowNum);
            return this;
        }

        public ValidationContext<TEntity, TResult> MaxValue(TResult maxValue)
        {
            if (typeof(TResult).IsAssignableFrom(typeof(IComparable)))
                m_Validator.MaxValue((IComparable)m_Property.Compile().Invoke(m_Entity), (IComparable)maxValue, GetPropertyName(), m_RowNum);
            return this;
        }

        public ValidationContext<TEntity, TResult> MinValue(TResult minValue)
        {
            if (typeof(TResult).IsAssignableFrom(typeof(IComparable)))
                m_Validator.MinValue((IComparable)m_Property.Compile().Invoke(m_Entity), (IComparable)minValue, GetPropertyName(), m_RowNum);
            return this;
        }

        public ValidationContext<TEntity, TResult> IsUnique(Predicate<TEntity> predicate)
        {
            if (m_Validator.IsValid)
                m_Validator.Assert(predicate.Invoke(m_Entity), GetPropertyName(), ErrorText.Exists, m_RowNum);
            return this;
        }

        delegate bool Func(TEntity model, bool isNew);
    }
}
