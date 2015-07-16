using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Memento
{
    public interface IExcutable
    {
        /// <summary>
        /// 重做
        /// </summary>
        void Redo();
        /// <summary>
        /// 撤销
        /// </summary>
        void Undo();
    }
}
