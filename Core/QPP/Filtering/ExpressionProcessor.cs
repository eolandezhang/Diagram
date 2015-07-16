using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QPP.Filtering;
using System.Linq.Expressions;
using System.Reflection;

namespace QPP.Filtering
{
    /// <summary>
    /// 從表達式構建CriteriaOperator
    /// </summary>
    internal static class ExpressionProcessor
    {
        private readonly static IDictionary<string, Func<MethodCallExpression, CriteriaOperator>> _customMethodCallProcessors
            = new Dictionary<string, Func<MethodCallExpression, CriteriaOperator>>();

        static ExpressionProcessor()
        {
            RegisterCustomMethodCall(() => FilteringExtensions.IsNull(null), FilteringExtensions.ProcessIsNull);
            RegisterCustomMethodCall(() => FilteringExtensions.IsLike("", ""), FilteringExtensions.ProcessIsLike);
            //RegisterCustomMethodCall(() => FilteringExtensions.IsLike("", "", null), FilteringExtensions.ProcessIsLikeMatchMode);
            //RegisterCustomMethodCall(() => FilteringExtensions.IsLike("", "", null, null), FilteringExtensions.ProcessIsLikeMatchModeEscapeChar);
            //RegisterCustomMethodCall(() => FilteringExtensions.IsInsensitiveLike("", ""), FilteringExtensions.ProcessIsInsensitiveLike);
            //RegisterCustomMethodCall(() => FilteringExtensions.IsInsensitiveLike("", "", null), FilteringExtensions.ProcessIsInsensitiveLikeMatchMode);
            RegisterCustomMethodCall(() => FilteringExtensions.IsIn(null, new object[0]), FilteringExtensions.ProcessIsInArray);
            RegisterCustomMethodCall(() => FilteringExtensions.IsIn(null, new List<object>()), FilteringExtensions.ProcessIsInCollection);
            //RegisterCustomMethodCall(() => FilteringExtensions.IsBetween(null, null).And(null), FilteringExtensions.ProcessIsBetween);
        }

        static void RegisterCustomMethodCall(Expression<Func<bool>> function, Func<MethodCallExpression, CriteriaOperator> functionProcessor)
        {
            MethodCallExpression functionExpression = (MethodCallExpression)function.Body;
            string signature = Signature(functionExpression.Method);
            _customMethodCallProcessors.Add(signature, functionProcessor);
        }

        public class ParameterOption
        {
            public bool WithParameter { get; set; }
            public Dictionary<Type, string> TypeFormat { get; set; }
        }

        public static CriteriaOperator ProcessLambdaExpression(LambdaExpression expression, bool withParameter = false)
        {
            ParameterOption option = new ParameterOption();
            option.WithParameter = withParameter;
            option.TypeFormat = new Dictionary<Type, string>();
            if (withParameter && expression.Parameters.Count > 0)
            {
                var p = expression.Parameters[0];
                option.TypeFormat.Add(p.Type, "domain.{0}");
            }
            return ProcessExpression(expression.Body, option);
        }

        public static string FindPropertyExpression(Expression expression)
        {
            ParameterOption option = new ParameterOption();
            return FindPropertyExpression(expression, option);
        }

        public static string FindPropertyExpression(Expression expression, ParameterOption option)
        {
            string memberExpression = FindMemberExpression(expression, option);
            int periodPosition = memberExpression.LastIndexOf('.') + 1;
            string property = (periodPosition <= 0) ? memberExpression : memberExpression.Substring(periodPosition);
            return property;
        }
        public static CriteriaOperator FindMemberOperator(Expression expression)
        {
            ParameterOption option = new ParameterOption();
            return FindMemberOperator(expression, option);
        }
        public static CriteriaOperator FindMemberOperator(Expression expression, ParameterOption option)
        {
            if (!IsMemberExpression(expression))
                return new OperandValue(FindValue(expression));

            if (expression is UnaryExpression)
            {
                UnaryExpression unaryExpression = (UnaryExpression)expression;

                if (unaryExpression.NodeType != ExpressionType.Convert)
                    throw new Exception("Cannot interpret member from " + expression.ToString());

                return FindMemberOperator(unaryExpression.Operand, option);
            }

            return new OperandProperty(FindMemberExpression(expression, option));
        }

        private static CriteriaOperator ProcessExpression(Expression expression, ParameterOption option)
        {
            if (expression is NewArrayExpression)
                return ProcessNewArrayExpression((NewArrayExpression)expression, option);
            if (expression is BinaryExpression)
                return ProcessBinaryExpression((BinaryExpression)expression, option);

            return ProcessBooleanExpression((Expression)expression, option);
        }

        private static CriteriaOperator ProcessNewArrayExpression(NewArrayExpression newArrayExpression, ParameterOption option)
        {
            var criteria = new GroupOperator(GroupOperatorType.Or);
            foreach (var e in newArrayExpression.Expressions)
                criteria.Operands.Add(FindMemberOperator(e, option));

            return criteria;
        }

        private static CriteriaOperator ProcessBooleanExpression(Expression expression, ParameterOption option)
        {
            if (expression is MemberExpression)
            {
                return new BinaryOperator(FindMemberExpression(expression, option), true);
            }

            if (expression is UnaryExpression)
            {
                UnaryExpression unaryExpression = (UnaryExpression)expression;

                if (unaryExpression.NodeType != ExpressionType.Not)
                    throw new Exception("Cannot interpret member from " + expression.ToString());

                if (IsMemberExpression(unaryExpression.Operand))
                    return new BinaryOperator(FindMemberExpression(unaryExpression.Operand, option), false);
                else
                    return ProcessExpression(unaryExpression.Operand, option).Not();
            }

            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression != null)
            {
                return ProcessCustomMethodCall(methodCallExpression);
            }

            if (expression is TypeBinaryExpression)
            {
                TypeBinaryExpression typeBinaryExpression = (TypeBinaryExpression)expression;
                return new BinaryOperator(ClassMember(typeBinaryExpression.Expression, option), typeBinaryExpression.TypeOperand.FullName);
            }

            throw new Exception("Could not determine member type from " + expression.NodeType + ", " + expression.ToString() + ", " + expression.GetType());
        }

        private static CriteriaOperator ProcessCustomMethodCall(MethodCallExpression methodCallExpression)
        {
            string signature = Signature(methodCallExpression.Method);

            Func<MethodCallExpression, CriteriaOperator> customMethodCallProcessor;
            if (!_customMethodCallProcessors.TryGetValue(signature, out customMethodCallProcessor))
                throw new Exception("Unrecognised method call: " + signature);

            return customMethodCallProcessor(methodCallExpression);
        }

        public static string Signature(MethodInfo methodInfo)
        {
            while (methodInfo.IsGenericMethod && !methodInfo.IsGenericMethodDefinition)
                methodInfo = methodInfo.GetGenericMethodDefinition();

            return methodInfo.DeclaringType.FullName + ":" + methodInfo;
        }

        private static CriteriaOperator ProcessBinaryExpression(BinaryExpression expression, ParameterOption option)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.AndAlso:
                    return ProcessAndExpression(expression, option);

                case ExpressionType.OrElse:
                    return ProcessOrExpression(expression, option);

                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    if (IsMemberExpression(expression.Right))
                        return ProcessMemberExpression(expression, option);
                    else
                        return ProcessSimpleExpression(expression, option);

                default:
                    throw new Exception("Unhandled binary expression: " + expression.NodeType + ", " + expression.ToString());
            }
        }

        private static CriteriaOperator ProcessSimpleExpression(BinaryExpression be, ParameterOption option)
        {
            if (be.Left.NodeType == ExpressionType.Call && ((MethodCallExpression)be.Left).Method.Name == "CompareString")
                return ProcessVisualBasicStringComparison(be, option);

            return ProcessSimpleExpression(be.Left, be.Right, be.NodeType, option);
        }

        private static CriteriaOperator ProcessSimpleExpression(Expression left, Expression right, ExpressionType nodeType, ParameterOption option)
        {
            CriteriaOperator property = FindMemberOperator(left, option);
            System.Type propertyType = FindMemberType(left);

            object value = FindValue(right);
            value = ConvertType(value, propertyType);

            return new BinaryOperator(property, new OperandValue(value), GetBinaryType(nodeType));
        }

        private static System.Type FindMemberType(Expression expression)
        {
            if (expression is MemberExpression)
            {
                MemberExpression memberExpression = (MemberExpression)expression;

                return memberExpression.Type;
            }

            if (expression is UnaryExpression)
            {
                UnaryExpression unaryExpression = (UnaryExpression)expression;

                if (unaryExpression.NodeType != ExpressionType.Convert)
                    throw new Exception("Cannot interpret member from " + expression.ToString());

                return FindMemberType(unaryExpression.Operand);
            }

            if (expression is MethodCallExpression)
            {
                var methodCallExpression = (MethodCallExpression)expression;
                return methodCallExpression.Method.ReturnType;
            }

            throw new Exception("Could not determine member type from " + expression.ToString());
        }

        public static object FindValue(Expression expression)
        {
            var valueExpression = Expression.Lambda(expression).Compile();
            object value = valueExpression.DynamicInvoke();
            return value;
        }

        private static string FindMemberExpression(Expression expression, ParameterOption option)
        {
            if (expression is MemberExpression)
            {
                MemberExpression memberExpression = (MemberExpression)expression;

                if (memberExpression.Expression.NodeType == ExpressionType.MemberAccess
                    || memberExpression.Expression.NodeType == ExpressionType.Call)
                {
                    if (IsNullableOfT(memberExpression.Member.DeclaringType))
                    {
                        // it's a Nullable<T>, so ignore any .Value
                        if (memberExpression.Member.Name == "Value")
                            return FindMemberExpression(memberExpression.Expression, new ParameterOption());
                    }
                    return FindMemberExpression(memberExpression.Expression, new ParameterOption()) + "." + memberExpression.Member.Name;
                    //return FindMemberExpression(memberExpression.Expression, option) + "." + memberExpression.Member.Name;
                }
                else if (memberExpression.Expression.NodeType == ExpressionType.Convert)
                {
                    return (FindMemberExpression(memberExpression.Expression, new ParameterOption()) + "." + memberExpression.Member.Name).TrimStart('.');
                }
                else if (option.WithParameter)
                {
                    if (option.TypeFormat.ContainsKey(memberExpression.Expression.Type))
                        return option.TypeFormat[memberExpression.Expression.Type].FormatArgs(memberExpression.Member.Name);
                    return (memberExpression.Expression + "." + memberExpression.Member.Name).TrimStart('.');
                }
                else
                {
                    return memberExpression.Member.Name;
                }
            }

            if (expression is UnaryExpression)
            {
                UnaryExpression unaryExpression = (UnaryExpression)expression;

                if (unaryExpression.NodeType != ExpressionType.Convert)
                    throw new Exception("Cannot interpret member from " + expression.ToString());

                return FindMemberExpression(unaryExpression.Operand, option);
            }

            if (expression is MethodCallExpression)
            {
                MethodCallExpression methodCallExpression = (MethodCallExpression)expression;

                if (methodCallExpression.Method.Name == "GetType")
                    return ClassMember(methodCallExpression.Object, option);

                if (methodCallExpression.Method.Name == "get_Item")
                    return FindMemberExpression(methodCallExpression.Object, option);

                if (methodCallExpression.Method.Name == "First")
                    return FindMemberExpression(methodCallExpression.Arguments[0], option);

                throw new Exception("Unrecognised method call in expression " + expression.ToString());
            }

            if (expression is ParameterExpression)
                return "";

            throw new Exception("Could not determine member from " + expression.ToString());
        }

        private static string ClassMember(Expression expression, ParameterOption option)
        {
            if (expression.NodeType == ExpressionType.MemberAccess)
                return FindMemberExpression(expression, option) + ".class";
            else
                return "class";
        }

        private static CriteriaOperator ProcessVisualBasicStringComparison(BinaryExpression be, ParameterOption option)
        {
            var methodCall = (MethodCallExpression)be.Left;

            if (IsMemberExpression(methodCall.Arguments[1]))
                return ProcessMemberExpression(methodCall.Arguments[0], methodCall.Arguments[1], be.NodeType, option);
            else
                return ProcessSimpleExpression(methodCall.Arguments[0], methodCall.Arguments[1], be.NodeType, option);
        }

        private static CriteriaOperator ProcessMemberExpression(BinaryExpression be, ParameterOption option)
        {
            return ProcessMemberExpression(be.Left, be.Right, be.NodeType, option);
        }

        private static BinaryOperatorType GetBinaryType(ExpressionType nodeType)
        {
            switch (nodeType)
            {
                case ExpressionType.Equal: return BinaryOperatorType.Equal;
                case ExpressionType.NotEqual: return BinaryOperatorType.NotEqual;
                case ExpressionType.GreaterThan: return BinaryOperatorType.Greater;
                case ExpressionType.GreaterThanOrEqual: return BinaryOperatorType.GreaterOrEqual;
                case ExpressionType.LessThan: return BinaryOperatorType.Less;
                case ExpressionType.LessThanOrEqual: return BinaryOperatorType.LessOrEqual;

                default:
                    throw new Exception("Unhandled property expression type: " + nodeType);
            }
        }

        private static CriteriaOperator ProcessMemberExpression(Expression left, Expression right, ExpressionType nodeType, ParameterOption option)
        {
            CriteriaOperator leftProperty = FindMemberOperator(left, option);
            CriteriaOperator rightProperty = FindMemberOperator(right, option);

            return new BinaryOperator(leftProperty, rightProperty, GetBinaryType(nodeType));
        }

        private static CriteriaOperator ProcessSimpleNullExpression(OperandProperty property, ExpressionType expressionType)
        {
            if (expressionType == ExpressionType.Equal)
                return property.IsNull();

            if (expressionType == ExpressionType.NotEqual)
                return property.IsNotNull();

            throw new Exception("Cannot supply null value to operator " + expressionType);
        }

        private static bool IsMemberExpression(Expression expression)
        {
            if (expression is ParameterExpression)
                return true;

            if (expression is MemberExpression)
            {
                MemberExpression memberExpression = (MemberExpression)expression;

                if (memberExpression.Expression == null)
                    return false;  // it's a member of a static class

                if (IsMemberExpression(memberExpression.Expression))
                    return true;

                // if the member has a null value, it was an alias
                return EvaluatesToNull(memberExpression.Expression);
            }

            if (expression is UnaryExpression)
            {
                UnaryExpression unaryExpression = (UnaryExpression)expression;

                if (unaryExpression.NodeType != ExpressionType.Convert)
                    throw new Exception("Cannot interpret member from " + expression.ToString());

                return IsMemberExpression(unaryExpression.Operand);
            }

            if (expression is MethodCallExpression)
            {
                MethodCallExpression methodCallExpression = (MethodCallExpression)expression;

                if (methodCallExpression.Method.Name == "First")
                {
                    if (IsMemberExpression(methodCallExpression.Arguments[0]))
                        return true;

                    return EvaluatesToNull(methodCallExpression.Arguments[0]);
                }

                if (methodCallExpression.Method.Name == "GetType"
                    || methodCallExpression.Method.Name == "get_Item")
                {
                    if (IsMemberExpression(methodCallExpression.Object))
                        return true;

                    return EvaluatesToNull(methodCallExpression.Object);
                }
            }

            return false;
        }

        private static bool EvaluatesToNull(Expression expression)
        {
            var valueExpression = Expression.Lambda(expression).Compile();
            object value = valueExpression.DynamicInvoke();
            return (value == null);
        }

        private static object ConvertType(object value, System.Type type)
        {
            if (value == null)
                return null;

            if (type.IsAssignableFrom(value.GetType()))
                return value;

            if (IsNullableOfT(type))
                type = Nullable.GetUnderlyingType(type);

            if (type.IsEnum)
                return Enum.ToObject(type, value);

            if (type.IsPrimitive)
                return Convert.ChangeType(value, type);

            throw new Exception("Cannot convert '" + value.ToString() + "' to " + type.ToString());
        }

        private static bool IsNullableOfT(System.Type type)
        {
            return type.IsGenericType
                && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        private static CriteriaOperator ProcessAndExpression(BinaryExpression expression, ParameterOption option)
        {
            return new GroupOperator(GroupOperatorType.And, ProcessExpression(expression.Left, option), ProcessExpression(expression.Right, option));
        }

        private static CriteriaOperator ProcessOrExpression(BinaryExpression expression, ParameterOption option)
        {
            return new GroupOperator(GroupOperatorType.Or, ProcessExpression(expression.Left, option), ProcessExpression(expression.Right, option));
        }
    }
}
