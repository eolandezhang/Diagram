using QPP.ComponentModel;
using QPP.Layout;
using QPP.Modularity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QPP.Runtime
{
    /// <summary>
    /// 運行時容器
    /// </summary>
    public interface IRuntimeContainer : ILoadableContainer, IPresenterContainer, IModularityContainer
    {
        /// <summary>
        /// 主視圖
        /// </summary>
        IPresenter MainView { get; }
        /// <summary>
        /// 運行
        /// </summary>
        void Run(IPresenter mainView);
        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns>序列化結果</returns>
        string Serialize();
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="layout">序列化結果</param>
        void Deserialize(string data);
        /// <summary>
        /// 設置佈局管理器
        /// </summary>
        void SetLayoutManager(ILayoutManager manager);
    }
}
