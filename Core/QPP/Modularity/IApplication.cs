using QPP.ComponentModel;
using QPP.Context;
using QPP.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Modularity
{
    public interface IApplication : ILoadableItem
    {
        /// <summary>
        /// 应用初始化，如应该上下文初始化。
        /// </summary>
        void Initialize();

        IAppContext Context { get; }

        IAppContext OriginalContext { get; }

        IAppContext CreateContext();

        /// <summary>
        /// 模擬上下文
        /// </summary>
        /// <param name="service"></param>
        void Simulate(IAppContext ctx);

        /// <summary>
        /// 重置為原始上下文
        /// </summary>
        void Reset();
    }
}
