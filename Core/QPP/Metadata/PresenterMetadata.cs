using QPP.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Metadata
{
    /// <summary>
    /// 視圖元數據
    /// </summary>
    public class PresenterMetadata
    {
        public PresenterMetadata()
        {
            Module = new ModuleMetadata();
            Properties = new NameObjectCollection();
        }
        /// <summary>
        /// 視圖的唯一代碼
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 模塊
        /// </summary>
        public ModuleMetadata Module { get; private set; }
        /// <summary>
        /// 视图的Uri
        /// </summary>
        public string Uri { get; set; }
        /// <summary>
        /// 視圖的擴展屬性
        /// </summary>
        public NameObjectCollection Properties { get; private set; }
    }
}
