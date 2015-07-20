using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace QPP.Command
{
    public class DataCommand : ICommand
    {
        Action<object[]> m_Redo;
        Action<object[]> m_Undo;
        object[] m_Datas;

        public DataCommand(Action<object[]> redo, Action<object[]> undo, params object[] datas)
        {
            m_Redo = redo;
            m_Undo = undo;
            m_Datas = datas;
        }

        public void Redo()
        {
            if (m_Redo != null)
                m_Redo.Invoke(m_Datas);
        }

        public void Undo()
        {
            if (m_Undo != null)
                m_Undo.Invoke(m_Datas);
        }
    }
}
