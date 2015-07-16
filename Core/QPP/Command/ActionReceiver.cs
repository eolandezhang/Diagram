using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Command
{
    public class ActionReceiver : AbstractReceiver
    {
        public Action Action { get; set; }

        public Func<bool> Executable { get; set; }

        public ActionReceiver(Action action, Func<bool> executable = null)
        {
            Action = action;
            Executable = executable;
        }

        public override bool CanExecute()
        {
            if (Executable == null)
                return true;
            return Executable.Invoke();
        }

        protected override void OnExecute()
        {
            Action.Invoke();
        }
    }

    public class ActionReceiver<T> : AbstractReceiver<T>
    {
        public Action<T> Action { get; set; }

        public Func<T, bool> Executable { get; set; }

        public ActionReceiver(Action<T> action, Func<T, bool> executable = null)
        {
            Action = action;
            Executable = executable;
        }

        public override bool CanExecute(T parameter)
        {
            if (Executable == null)
                return true;
            return Executable.Invoke(parameter);
        }

        protected override void OnExecute(T parameter)
        {
            Action.Invoke(parameter);
        }
    }
}
