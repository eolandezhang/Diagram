using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Linq.Expressions;
using QPP.Utils;

namespace QPP.Filtering
{
    public static class FilteringExtensions
    {
        /// <summary>
        /// Apply a "like" BinaryOperator in a CreateCriteria expression
        /// Note: throws an exception outside of a CreateCriteria expression
        /// </summary>
        public static bool IsNull(this object projection)
        {
            throw new Exception("Not to be used directly - use inside CreateCriteria expression");
        }
        /// <summary>
        /// Apply a "like" BinaryOperator in a CreateCriteria expression
        /// Note: throws an exception outside of a CreateCriteria expression
        /// </summary>
        public static bool IsLike(this string projection, string comparison)
        {
            throw new Exception("Not to be used directly - use inside CreateCriteria expression");
        }

        ///// <summary>
        ///// Apply a "like" restriction in a QueryOver expression
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static bool IsInsensitiveLike(this string projection, string comparison)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        /// <summary>
        /// Apply an "in" constraint to the named property 
        /// Note: throws an exception outside of a QueryOver expression
        /// </summary>
        public static bool IsIn(this object projection, object[] values)
        {
            throw new Exception("Not to be used directly - use inside QueryOver expression");
        }

        /// <summary>
        /// Apply an "in" constraint to the named property 
        /// Note: throws an exception outside of a QueryOver expression
        /// </summary>
        public static bool IsIn(this object projection, ICollection values)
        {
            throw new Exception("Not to be used directly - use inside QueryOver expression");
        }

        /// <summary>
        /// Apply a "between" constraint to the named property
        /// Note: throws an exception outside of a QueryOver expression
        /// </summary>
        public static RestrictionBetweenBuilder IsBetween(this object projection, object lo)
        {
            throw new Exception("Not to be used directly - use inside QueryOver expression");
        }

        public class RestrictionBetweenBuilder
        {
            public bool And(object hi)
            {
                throw new Exception("Not to be used directly - use inside QueryOver expression");
            }
        }

        public static CriteriaOperator ProcessIsLike(MethodCallExpression methodCallExpression)
        {
            var property = ExpressionProcessor.FindMemberOperator(methodCallExpression.Arguments[0]);
            object value = ExpressionProcessor.FindValue(methodCallExpression.Arguments[1]);
            return new BinaryOperator(property, new OperandValue(value), BinaryOperatorType.Like);
        }

        public static CriteriaOperator ProcessIsNull(MethodCallExpression methodCallExpression)
        {
            var property = ExpressionProcessor.FindMemberOperator(methodCallExpression.Arguments[0]);
            return new NullOperator(property);
        }

        //public static ICriterion ProcessIsInsensitiveLike(MethodCallExpression methodCallExpression)
        //{
        //    ExpressionProcessor.ProjectionInfo projection = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]);
        //    object value = ExpressionProcessor.FindValue(methodCallExpression.Arguments[1]);
        //    return projection.CreateCriterion(Restrictions.InsensitiveLike, Restrictions.InsensitiveLike, value);
        //}

        //public static ICriterion ProcessIsInsensitiveLikeMatchMode(MethodCallExpression methodCallExpression)
        //{
        //    ExpressionProcessor.ProjectionInfo projection = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]);
        //    string value = (string)ExpressionProcessor.FindValue(methodCallExpression.Arguments[1]);
        //    MatchMode matchMode = (MatchMode)ExpressionProcessor.FindValue(methodCallExpression.Arguments[2]);
        //    return projection.Create<ICriterion>(s => Restrictions.InsensitiveLike(s, value, matchMode), p => Restrictions.InsensitiveLike(p, value, matchMode));
        //}

        public static CriteriaOperator ProcessIsInArray(MethodCallExpression methodCallExpression)
        {
            var property = ExpressionProcessor.FindMemberOperator(methodCallExpression.Arguments[0]);
            object[] value = (object[])ExpressionProcessor.FindValue(methodCallExpression.Arguments[1]);
            List<OperandValue> o = new List<OperandValue>(value.Length);
            foreach (var v in value)
                o.Add(new OperandValue(v));
            return new InOperator(property, o.ToArray());
        }

        public static CriteriaOperator ProcessIsInCollection(MethodCallExpression methodCallExpression)
        {
            var property = ExpressionProcessor.FindMemberOperator(methodCallExpression.Arguments[0]);
            var value = (ICollection)ExpressionProcessor.FindValue(methodCallExpression.Arguments[1]);
            List<OperandValue> o = new List<OperandValue>(value.Count);
            foreach (var v in value)
                o.Add(new OperandValue(v));
            return new InOperator(property, o.ToArray());
        }

        //public static ICriterion ProcessIsBetween(MethodCallExpression methodCallExpression)
        //{
        //    MethodCallExpression betweenFunction = (MethodCallExpression)methodCallExpression.Object;
        //    ExpressionProcessor.ProjectionInfo projection = ExpressionProcessor.FindMemberProjection(betweenFunction.Arguments[0]);
        //    object lo = ExpressionProcessor.FindValue(betweenFunction.Arguments[1]);
        //    object hi = ExpressionProcessor.FindValue(methodCallExpression.Arguments[0]);
        //    return projection.Create<ICriterion>(s => Restrictions.Between(s, lo, hi), p => Restrictions.Between(p, lo, hi));
        //}
        ///// <summary>
        ///// Project SQL function year()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static int YearPart(this DateTime dateTimeProperty)
        //{
        //    throw new Exception("Not to be used directly - use inside CreateCriteria expression");
        //}

        //internal static IProjection ProcessYearPart(MethodCallExpression methodCallExpression)
        //{
        //    IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
        //    return Projections.SqlFunction("year", NHibernateUtil.Int32, property);
        //}

        ///// <summary>
        ///// Project SQL function day()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static int DayPart(this DateTime dateTimeProperty)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        //internal static IProjection ProcessDayPart(MethodCallExpression methodCallExpression)
        //{
        //    IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
        //    return Projections.SqlFunction("day", NHibernateUtil.Int32, property);
        //}

        ///// <summary>
        ///// Project SQL function month()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static int MonthPart(this DateTime dateTimeProperty)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        //internal static IProjection ProcessMonthPart(MethodCallExpression methodCallExpression)
        //{
        //    IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
        //    return Projections.SqlFunction("month", NHibernateUtil.Int32, property);
        //}

        ///// <summary>
        ///// Project SQL function hour()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static int HourPart(this DateTime dateTimeProperty)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        //internal static IProjection ProcessHourPart(MethodCallExpression methodCallExpression)
        //{
        //    IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
        //    return Projections.SqlFunction("hour", NHibernateUtil.Int32, property);
        //}

        ///// <summary>
        ///// Project SQL function minute()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static int MinutePart(this DateTime dateTimeProperty)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        //internal static IProjection ProcessMinutePart(MethodCallExpression methodCallExpression)
        //{
        //    IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
        //    return Projections.SqlFunction("minute", NHibernateUtil.Int32, property);
        //}

        ///// <summary>
        ///// Project SQL function second()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static int SecondPart(this DateTime dateTimeProperty)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        //internal static IProjection ProcessSecondPart(MethodCallExpression methodCallExpression)
        //{
        //    IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
        //    return Projections.SqlFunction("second", NHibernateUtil.Int32, property);
        //}

        ///// <summary>
        ///// Project SQL function sqrt()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static double Sqrt(this double numericProperty)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        ///// <summary>
        ///// Project SQL function sqrt()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static double Sqrt(this int numericProperty)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        ///// <summary>
        ///// Project SQL function sqrt()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static double Sqrt(this long numericProperty)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        ///// <summary>
        ///// Project SQL function sqrt()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static double Sqrt(this decimal numericProperty)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        ///// <summary>
        ///// Project SQL function sqrt()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static double Sqrt(this byte numericProperty)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        //internal static IProjection ProcessSqrt(MethodCallExpression methodCallExpression)
        //{
        //    IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
        //    return Projections.SqlFunction("sqrt", NHibernateUtil.Double, property);
        //}

        ///// <summary>
        ///// Project SQL function lower()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static string Lower(this string stringProperty)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        //internal static IProjection ProcessLower(MethodCallExpression methodCallExpression)
        //{
        //    IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
        //    return Projections.SqlFunction("lower", NHibernateUtil.String, property);
        //}

        ///// <summary>
        ///// Project SQL function upper()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static string Upper(this string stringProperty)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        //internal static IProjection ProcessUpper(MethodCallExpression methodCallExpression)
        //{
        //    IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
        //    return Projections.SqlFunction("upper", NHibernateUtil.String, property);
        //}

        ///// <summary>
        ///// Project SQL function abs()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static int Abs(this int numericProperty)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        //internal static IProjection ProcessIntAbs(MethodCallExpression methodCallExpression)
        //{
        //    IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
        //    return Projections.SqlFunction("abs", NHibernateUtil.Int32, property);
        //}

        ///// <summary>
        ///// Project SQL function abs()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static Int64 Abs(this Int64 numericProperty)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        //internal static IProjection ProcessInt64Abs(MethodCallExpression methodCallExpression)
        //{
        //    IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
        //    return Projections.SqlFunction("abs", NHibernateUtil.Int64, property);
        //}

        ///// <summary>
        ///// Project SQL function abs()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static double Abs(this double numericProperty)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        //internal static IProjection ProcessDoubleAbs(MethodCallExpression methodCallExpression)
        //{
        //    IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
        //    return Projections.SqlFunction("abs", NHibernateUtil.Double, property);
        //}

        ///// <summary>
        ///// Project SQL function trim()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static string TrimStr(this string stringProperty)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        //internal static IProjection ProcessTrimStr(MethodCallExpression methodCallExpression)
        //{
        //    IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
        //    return Projections.SqlFunction("trim", NHibernateUtil.String, property);
        //}

        ///// <summary>
        ///// Project SQL function length()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static int StrLength(this string stringProperty)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        //internal static IProjection ProcessStrLength(MethodCallExpression methodCallExpression)
        //{
        //    IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
        //    return Projections.SqlFunction("length", NHibernateUtil.String, property);
        //}

        ///// <summary>
        ///// Project SQL function bit_length()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static int BitLength(this string stringProperty)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        //internal static IProjection ProcessBitLength(MethodCallExpression methodCallExpression)
        //{
        //    IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
        //    return Projections.SqlFunction("bit_length", NHibernateUtil.String, property);
        //}

        ///// <summary>
        ///// Project SQL function substring()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static string Substr(this string stringProperty, int startIndex, int length)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        //internal static IProjection ProcessSubstr(MethodCallExpression methodCallExpression)
        //{
        //    IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
        //    object startIndex = ExpressionProcessor.FindValue(methodCallExpression.Arguments[1]);
        //    object length = ExpressionProcessor.FindValue(methodCallExpression.Arguments[2]);
        //    return Projections.SqlFunction("substring", NHibernateUtil.String, property, Projections.Constant(startIndex), Projections.Constant(length));
        //}

        ///// <summary>
        ///// Project SQL function locate()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static int CharIndex(this string stringProperty, string theChar, int startLocation)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        //internal static IProjection ProcessCharIndex(MethodCallExpression methodCallExpression)
        //{
        //    IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
        //    object theChar = ExpressionProcessor.FindValue(methodCallExpression.Arguments[1]);
        //    object startLocation = ExpressionProcessor.FindValue(methodCallExpression.Arguments[2]);
        //    return Projections.SqlFunction("locate", NHibernateUtil.String, Projections.Constant(theChar), property, Projections.Constant(startLocation));
        //}

        ///// <summary>
        ///// Project SQL function coalesce()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static T Coalesce<T>(this T objectProperty, T replaceValueIfIsNull)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        ///// <summary>
        ///// Project SQL function coalesce()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static T? Coalesce<T>(this T? objectProperty, T replaceValueIfIsNull) where T : struct
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        //internal static IProjection ProcessCoalesce(MethodCallExpression methodCallExpression)
        //{
        //    IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
        //    object replaceValueIfIsNull = ExpressionProcessor.FindValue(methodCallExpression.Arguments[1]);
        //    return Projections.SqlFunction("coalesce", NHibernateUtil.Object, property, Projections.Constant(replaceValueIfIsNull));
        //}

        ///// <summary>
        ///// Project SQL function mod()
        ///// Note: throws an exception outside of a QueryOver expression
        ///// </summary>
        //public static int Mod(this int numericProperty, int divisor)
        //{
        //    throw new Exception("Not to be used directly - use inside QueryOver expression");
        //}

        //internal static IProjection ProcessMod(MethodCallExpression methodCallExpression)
        //{
        //    IProjection property = ExpressionProcessor.FindMemberProjection(methodCallExpression.Arguments[0]).AsProjection();
        //    object divisor = ExpressionProcessor.FindValue(methodCallExpression.Arguments[1]);
        //    return Projections.SqlFunction("mod", NHibernateUtil.Int32, property, Projections.Constant(divisor));
        //}
    }
}
