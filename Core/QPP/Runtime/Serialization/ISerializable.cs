using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Runtime.Serialization
{
    public interface ISerializable
    {
        /// <summary>
        /// 是否能序列化
        /// </summary>
        bool CanSerialize { get; set; }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="e"></param>
        void Serialize(SerializationInfo info);
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="e"></param>
        void Deserialize(SerializationInfo info);
    }
}
