namespace QPP.Wpf.UI.Converters.Expressions.Nodes
{
    using System.Windows;

    // a node to negate a boolean value
    internal sealed class NotNode : UnaryNode
    {
        //private static readonly ExceptionHelper exceptionHelper = new ExceptionHelper(typeof(NotNode));

        public NotNode(Node node)
            : base(node)
        {
        }

        public override object Evaluate(NodeEvaluationContext evaluationContext)
        {
            var value = Node.Evaluate(evaluationContext);

            if (value == DependencyProperty.UnsetValue)
            {
                return DependencyProperty.UnsetValue;
            }

            var nodeValueType = GetNodeValueType(value);
            if (nodeValueType != NodeValueType.Boolean)
                throw new ParseException("Operator '!' cannot be applied to operand of type '{0}'.".FormatArgs(nodeValueType));
            //exceptionHelper.ResolveAndThrowIf(nodeValueType != NodeValueType.Boolean, "NotBooleanType", nodeValueType);
            return !((bool)value);
        }
    }
}
