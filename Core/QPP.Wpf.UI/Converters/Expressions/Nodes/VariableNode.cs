namespace QPP.Wpf.UI.Converters.Expressions.Nodes
{
    using System.Diagnostics;

    // node to hold a reference to a variable
    internal sealed class VariableNode : Node
    {
        //private static readonly ExceptionHelper exceptionHelper = new ExceptionHelper(typeof(VariableNode));

        private readonly int index;

        public VariableNode(int index)
        {
            Debug.Assert(index >= 0);
            this.index = index;
        }

        public override object Evaluate(NodeEvaluationContext evaluationContext)
        {
            Debug.Assert(evaluationContext != null);
            if (!evaluationContext.HasArgument(this.index))
                throw new ParseException("No argument with index {0} has been supplied.".FormatArgs(this.index));

            //exceptionHelper.ResolveAndThrowIf(!evaluationContext.HasArgument(this.index), "ArgumentNotFound", this.index);

            // variable values are passed inside the context for each evaluation
            return evaluationContext.GetArgument(this.index);
        }
    }
}
