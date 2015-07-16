using QPP.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Runtime
{
    /// <summary>
    /// 運行時可加載項目加載事件參數
    /// </summary>
    public class LoadEventArgs: EventArgs
    {
        /// <summary>
        /// 運行時可加載項目
        /// </summary>
        public ILoadableItem Item { get; private set; }

        public LoadEventArgs(ILoadableItem item)
        {
            Item = item;
        }
    }
}
