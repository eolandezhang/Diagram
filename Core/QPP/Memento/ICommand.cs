using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Command
{
    public interface ICommand
    {
        //ActionHandler DoOperation { get; set; }
        //ActionHandler UndoOperation { get; set; }

        void Redo();
        void Undo();
    }
}
