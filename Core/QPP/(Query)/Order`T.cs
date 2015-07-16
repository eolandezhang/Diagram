using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using QPP.Filtering;

namespace QPP
{
    public enum OrderType { ASC, DESC }

    public class Order
    {
        public static Order<T> DESC<T>(Expression<Func<T, object>> expr)
        {
            return new Order<T>().DESC(expr);
        }

        public static Order<T> ASC<T>(Expression<Func<T, object>> expr)
        {
            return new Order<T>().ASC(expr);
        }
    }

    public class Order<T>
    {
        List<KeyValuePair<Expression<Func<T, object>>, OrderType>> orders
            = new List<KeyValuePair<Expression<Func<T, object>>, OrderType>>();

        public override string ToString()
        {
            string order = "";
            foreach (var o in orders)
            {
                string property = ExpressionProcessor.FindPropertyExpression(o.Key.Body);
                order += property + " " + o.Value + ",";
            }
            return order.TrimEnd(',');
        }

        public virtual string Value
        {
            get { return ToString(); }
        }

        public Order<T> DESC(Expression<Func<T, object>> expr)
        {
            return Add(expr, OrderType.DESC);
        }

        public Order<T> ASC(Expression<Func<T, object>> expr)
        {
            return Add(expr, OrderType.ASC);
        }

        public Order<T> Add(Expression<Func<T, object>> expr, OrderType type)
        {
            orders.Add(new KeyValuePair<Expression<Func<T, object>>, OrderType>(expr, type));
            return this;
        }
    }
}
