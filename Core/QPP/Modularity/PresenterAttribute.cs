using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Modularity
{
    /// <summary>
    /// 視圖元數據描述
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PresenterAttribute : Attribute
    {
        /// <summary>
        /// 模塊
        /// </summary>
        public Enum Module { get; private set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 视图的Uri
        /// </summary>
        public string Uri { get; set; }

        public PresenterAttribute()
        {

        }

        /// <summary>
        /// 視圖元數據的構造器
        /// </summary>
        /// <param name="module">枚舉類型的模塊元數據定義</param>
        public PresenterAttribute(object module)
        {
            Module = (Enum)module;
        }
    }
}
