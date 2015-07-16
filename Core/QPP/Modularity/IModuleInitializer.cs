using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Modularity
{
    /// <summary>
    /// 模塊初始化接口，程序集加載到AppDomain時，執行初始化
    /// </summary>
    public interface IModuleInitializer
    {
        /// <summary>
        /// 模组初始化。
        /// </summary>
        void Initialize();
    }
}
