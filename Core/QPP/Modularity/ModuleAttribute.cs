using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Modularity
{
    /// <summary>
    /// 模塊元數據描述
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ModuleAttribute : Attribute
    {
        /// <summary>
        /// 模塊
        /// </summary>
        public Enum Module { get; private set; }

        /// <summary>
        /// 視圖元數據的構造器
        /// </summary>
        /// <param name="module">枚舉類型的模塊元數據定義</param>
        public ModuleAttribute(object module)
        {
            Module = (Enum)module;
        }
    }
}
