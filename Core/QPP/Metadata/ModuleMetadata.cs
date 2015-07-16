using QPP.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Metadata
{
    /// <summary>
    /// 模塊元數據
    /// </summary>
    public class ModuleMetadata
    {
        public ModuleMetadata()
        {
            Properties = new NameObjectCollection();
        }
        /// <summary>
        /// 模塊的類名
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 模塊類型
        /// </summary>
        public Enum Type { get; set; }
        /// <summary>
        /// 模塊的擴展屬性
        /// </summary>
        public NameObjectCollection Properties { get; set; }
        /// <summary>
        /// 所屬應用(IApplication)的類型
        /// </summary>
        public Type AppType { get; set; }
    }
}
