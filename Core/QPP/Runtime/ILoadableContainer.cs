using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Runtime
{
    /// <summary>
    /// 可加載項目容器
    /// </summary>
    public interface ILoadableContainer
    {
        /// <summary>
        /// 運行時可加載項目加載事件
        /// </summary>
        event EventHandler<LoadEventArgs> ItemLoaded;
        /// <summary>
        /// 運行時可加載項目卸載事件
        /// </summary>
        event EventHandler<LoadEventArgs> ItemUnLoaded;
        /// <summary>
        /// 載入項目
        /// </summary>
        /// <param name="item"></param>
        void Load(ILoadableItem item);
        /// <summary>
        /// 卸載項目
        /// </summary>
        /// <param name="item"></param>
        void UnLoad(ILoadableItem item);
    }
}
