using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.UI.Controls.XGantt
{
    /// <summary>
    /// A drag state determines how the mouse move events should act on a GanttItem
    /// </summary>
    public enum DragState
    {
        /// <summary>
        /// Do not move the item
        /// </summary>
        None,
        /// <summary>
        /// Move the entire item.
        /// </summary>
        Whole,
        /// <summary>
        /// The right side of the item is being drug.
        /// </summary>
        ResizeRight,
        ///// <summary>
        ///// The left side of the item is being drug.
        ///// </summary>
        //ResizeLeft
    }
}
