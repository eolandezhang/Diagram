using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Command
{
    /// <summary>
    /// 事件命令
    /// </summary>
    public enum EventCommands
    {
        /// <summary>
        /// 加載
        /// </summary>
        Loaded,
        /// <summary>
        /// 關閉時
        /// </summary>
        Closing,
        /// <summary>
        /// 關閉后
        /// </summary>
        Closed,
        /// <summary>
        /// 序列化
        /// </summary>
        Serializing,
        /// <summary>
        /// 反序列化
        /// </summary>
        Deserializing,
    }
}
