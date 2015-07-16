using QPP.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Runtime
{
    /// <summary>
    /// 模塊化容器
    /// </summary>
    public interface IModularityContainer
    {
        /// <summary>
        /// 應用列舉
        /// </summary>
        IEnumerable<IApplication> Applications { get; }
        /// <summary>
        /// 模塊列舉
        /// </summary>
        IEnumerable<IModule> Modules { get; }
        /// <summary>
        /// 獲取視圖所屬的應用
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        IApplication GetApplication(IPresenter presenter);
        /// <summary>
        /// 獲取模塊所屬的應用
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        IApplication GetApplication(IModule module);
        /// <summary>
        /// 獲取視圖所屬的模塊
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        IModule GetModule(IPresenter presenter);
        /// <summary>
        /// 獲取應用已加載的模塊
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        IEnumerable<IModule> GetLoadedModules(IApplication application);
        /// <summary>
        /// 獲取模塊已加載的視圖
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        IEnumerable<IPresenter> GetLoadedPresenters(IModule module);
        /// <summary>
        /// 獲取應用已加載的視圖
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        IEnumerable<IPresenter> GetLoadedPresenters(IApplication app);
    }
}
