using QPP.ComponentModel;
using QPP.Layout;
using QPP.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Navigation
{
    public interface INavigator
    {
        /// <summary>
        /// 前進
        /// </summary>
        void Forward();
        /// <summary>
        /// 是否可以前進
        /// </summary>
        bool CanForward { get; }
        /// <summary>
        /// 后退
        /// </summary>
        void Backward();
        /// <summary>
        /// 是否可以后退
        /// </summary>
        bool CanBackward { get; }
        /// <summary>
        /// 添加訪問歷史
        /// </summary>
        void AddHistory(string caption, string uri, string view, string viewType);
        /// <summary>
        /// 添加修改記錄，當業務對象保存時，記錄到最近修改
        /// </summary>
        void AddModified(string caption, string hashCode, string uri, string viewTypeName, ViewType viewType);
        /// <summary>
        /// 刪除修改記錄，當業務對象被刪除時，刪除相關記錄
        /// </summary>
        void RemoveModified(string hashCode, string viewTypeName, ViewType viewType);
    }
}
