namespace QPP.Wpf.UI.Converters.Expressions.Nodes
{
    using System.Diagnostics;
    using System.Windows;

    // a node to complement an integral value
    internal sealed class ComplementNode : UnaryNode
    {
        //private static readonly ExceptionHelper exceptionHelper = new ExceptionHelper(typeof(ComplementNode));

        public ComplementNode(Node node)
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
            if (!Node.IsIntegralNodeValueType(nodeValueType))
                throw new ParseException("Operator '~' cannot be applied to operand of type '{0}'.".FormatArgs(nodeValueType));
            //exceptionHelper.ResolveAndThrowIf(!Node.IsIntegralNodeValueType(nodeValueType), "NotIntegralType", nodeValueType);

            switch (nodeValueType)
            {
                case NodeValueType.Byte:
                    return ~((byte)value);
                case NodeValueType.Int16:
                    return ~((short)value);
                case NodeValueType.Int32:
                    return ~((int)value);
                case NodeValueType.Int64:
                    return ~((long)value);
            }

            Debug.Assert(false);
            return null;
        }
    }
}
