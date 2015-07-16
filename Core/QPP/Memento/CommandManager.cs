using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Memento
{
    public class CommandManager
    {
        private Stack<IExcutable> redoMementos;

        private Stack<IExcutable> undoMementos;

        public CommandManager()
        {
            redoMementos = new Stack<IExcutable>();
            undoMementos = new Stack<IExcutable>();
        }

        /// <summary>
        /// 先清空Redo栈，将Command对象压入栈
        /// </summary>
        /// <param name="command">Command对象</param>
        public void AddCommand(IExcutable command)
        {
            redoMementos.Clear();
            undoMementos.Push(command);
        }

        /// <summary>
        /// 从Redo栈中弹出一个Command对象，执行Do操作，并且将该Command对象压入Undo栈
        /// </summary>
        public void Redo()
        {
            try
            {
                if (redoMementos.Count > 0)
                {
                    IExcutable command = redoMementos.Pop();
                    if (command != null)
                    {
                        command.Redo();
                        undoMementos.Push(command);
                    }
                }
            }
            catch (InvalidOperationException invalidOperationException)
            {
                Console.WriteLine(invalidOperationException.Message);
            }
        }

        /// <summary>
        /// 从Undo栈中弹出一个Command对象，执行Undo操作，并且将该Command对象压入Redo栈
        /// </summary>
        public void Undo()
        {
            try
            {
                if (undoMementos.Count > 0)
                {
                    IExcutable command = undoMementos.Pop();
                    if (command != null)
                    {
                        command.Undo();
                        redoMementos.Push(command);
                    }
                }
            }
            catch (InvalidOperationException invalidOperationException)
            {
                Console.WriteLine(invalidOperationException.Message);
            }
        }

        /// <summary>
        /// 清空Redo栈与Undo栈
        /// </summary>
        public void ClearAll()
        {
            ClearRedoStack();
            ClearUndoStack();
        }

        /// <summary>
        /// 清空Redo栈
        /// </summary>
        public void ClearRedoStack()
        {
            redoMementos.Clear();
        }

        /// <summary>
        /// 清空Undo栈
        /// </summary>
        public void ClearUndoStack()
        {
            undoMementos.Clear();
        }

        /// <summary>
        /// 当前在Redo栈中能重做的步骤数
        /// </summary>
        public int RedoStepsCount
        {
            get { return redoMementos.Count; }
        }

        /// <summary>
        /// 当前在Undo栈中能撤消的步骤数
        /// </summary>
        public int UndoStepsCount
        {
            get { return undoMementos.Count; }
        }

        public bool CanRedo
        {
            get { return RedoStepsCount > 0; }
        }

        public bool CanUndo
        {
            get { return UndoStepsCount > 0; }
        }
    }
}
