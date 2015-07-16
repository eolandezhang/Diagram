using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Command
{
    public interface IReceiver
    {
        event EventHandler Executing;
        event EventHandler Executed;
        void Execute();
        bool CanExecute();
    }

    public interface IReceiver<T>
    {
        event EventHandler<CommandEventArgs> Executing;
        event EventHandler<CommandEventArgs> Executed;

        void Execute(T model);
        bool CanExecute(T model);
    }
}
