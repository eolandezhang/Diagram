using QPP.Filtering;
using QPP.Filtering.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Filtering
{
    public class CriteriaToHqlWithParametersProcessor : CriteriaToStringBase
    {
        public const string ParameterPrefix = ":";
        int parameterIndex = 0;
        protected Dictionary<string, object> Parameters = new Dictionary<string, object>();

        protected CriteriaToHqlWithParametersProcessor()
        {
        }

        public override string GetOperatorString(UnaryOperatorType opType)
        {
            return CriteriaToBasicStyleParameterlessProcessor.GetBasicOperatorString(opType);
        }
        public override string GetOperatorString(BinaryOperatorType opType)
        {
            return CriteriaToBasicStyleParameterlessProcessor.GetBasicOperatorString(opType);
        }
        public override string GetOperatorString(GroupOperatorType opType)
        {
            return CriteriaToBasicStyleParameterlessProcessor.GetBasicOperatorString(opType);
        }

        public override object Visit(OperandProperty operand)
        {
            string result = operand.PropertyName;
            if (result == null)
                result = string.Empty;
            return new CriteriaToStringVisitResult(result);
        }

        public override object Visit(Filtering.OperandValue operand)
        {
            var parameterName = "_" + parameterIndex++;
            Parameters.Add(parameterName, operand.Value);
            return new CriteriaToStringVisitResult(ParameterPrefix + parameterName);
        }
        public static string ToString(CriteriaOperator criteria, out Dictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();
            if (ReferenceEquals(criteria, null))
            {
                return string.Empty;
            }
            CriteriaToHqlWithParametersProcessor processor = new CriteriaToHqlWithParametersProcessor();
            CriteriaToStringVisitResult visitResult = processor.Process(criteria);
            foreach (var p in processor.Parameters)
                parameters.Add(p.Key, p.Value);
            return visitResult.Result;
        }
    }
}
