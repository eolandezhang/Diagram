using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Runtime.Serialization
{
    /// <summary>
    /// 序列化事件參數
    /// </summary>
    public class SerializationEventArgs : EventArgs
    {
        /// <summary>
        /// 序列化的結果數據
        /// </summary>
        public SerializationInfo Info { get; private set; }
        /// <summary>
        /// 源對象
        /// </summary>
        public object Source { get; private set; }

        public SerializationEventArgs(object source, SerializationInfo info)
        {
            Source = source;
            Info = info;
        }
    }
}
