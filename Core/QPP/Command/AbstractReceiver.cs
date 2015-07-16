using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Command
{
    public abstract class AbstractReceiver : IReceiver
    {
        public event EventHandler Executing;

        public event EventHandler Executed;

        public void Execute()
        {
            OnExecuting(EventArgs.Empty);
            OnExecute();
            OnExecuted(EventArgs.Empty);
        }

        public abstract bool CanExecute();

        protected abstract void OnExecute();

        protected virtual void OnExecuting(EventArgs e)
        {
            if (Executing != null)
                Executing(this, e);
        }

        protected virtual void OnExecuted(EventArgs e)
        {
            if (Executed != null)
                Executed(this, e);
        }
    }

    public abstract class AbstractReceiver<T> : IReceiver<T>
    {
        public event EventHandler<CommandEventArgs> Executing;

        public event EventHandler<CommandEventArgs> Executed;

        public void Execute(T parameter)
        {
            var e = new CommandEventArgs(parameter);
            OnExecuting(e);
            OnExecute(parameter);
            OnExecuted(e);
        }

        public abstract bool CanExecute(T parameter);

        protected abstract void OnExecute(T parameter);

        protected virtual void OnExecuting(CommandEventArgs e)
        {
            if (Executing != null)
                Executing(this, e);
        }

        protected virtual void OnExecuted(CommandEventArgs e)
        {
            if (Executed != null)
                Executed(this, e);
        }
    }
}
