namespace QPP.Wpf.UI.Converters.Expressions.Nodes
{
    using System.Windows;

    internal sealed class TernaryConditionalNode : TernaryNode
    {
        //private static readonly ExceptionHelper exceptionHelper = new ExceptionHelper(typeof(TernaryConditionalNode));

        public TernaryConditionalNode(Node firstNode, Node secondNode, Node thirdNode)
            : base(firstNode, secondNode, thirdNode)
        {
        }

        protected override string OperatorSymbols
        {
            get { return "?"; }
        }

        public override object Evaluate(NodeEvaluationContext evaluationContext)
        {
            var firstNodeValue = this.FirstNode.Evaluate(evaluationContext);

            if (firstNodeValue == DependencyProperty.UnsetValue)
            {
                return DependencyProperty.UnsetValue;
            }

            var firstNodeValueType = GetNodeValueType(firstNodeValue);

            if (firstNodeValueType != NodeValueType.Boolean)
                throw new ParseException("Operator '{0}' requires that the first node be of type Boolean, but it is of type '{1}'.".FormatArgs(this.OperatorSymbols, firstNodeValueType));
            //exceptionHelper.ResolveAndThrowIf(firstNodeValueType != NodeValueType.Boolean, "FirstNodeMustBeBoolean", this.OperatorSymbols, firstNodeValueType);

            if ((bool)firstNodeValue)
            {
                return this.SecondNode.Evaluate(evaluationContext);
            }
            else
            {
                return this.ThirdNode.Evaluate(evaluationContext);
            }
        }
    }
}
