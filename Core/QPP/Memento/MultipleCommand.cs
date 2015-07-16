using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Memento
{
    public class MultipleCommand : IExcutable
    {
        List<IExcutable> m_Commands = new List<IExcutable>();

        public MultipleCommand(IEnumerable<IExcutable> commands)
        {
            m_Commands.AddRange(commands);
        }

        public MultipleCommand()
        {
        }

        public MultipleCommand AddCommand(IExcutable command)
        {
            m_Commands.Add(command);
            return this;
        }

        public void Redo()
        {
            foreach (var c in m_Commands)
                c.Redo();
        }

        public void Undo()
        {
            foreach (var c in m_Commands)
                c.Undo();
        }
    }
}
