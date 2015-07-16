using System;

namespace QPP.ComponentModel
{
    /// <summary>
    /// 有状态的对象
    /// </summary>
    public interface IStatefulObject
    {
        event EventHandler StateChanged;
        /// <summary>
        /// 状态
        /// </summary>
        DataState DataState { get; }

        /// <summary>
        /// Change the DataState to Created.
        /// </summary>
        void MarkCreated();

        /// <summary>
        /// Change the DataState to Modified.
        /// </summary>
        void MarkModified();

        /// <summary>
        /// Change the DataState to Deleted.
        /// </summary>
        void MarkDeleted();

        /// <summary>
        /// Change the DataState to None.
        /// </summary>
        void ResetState();
    }
}
