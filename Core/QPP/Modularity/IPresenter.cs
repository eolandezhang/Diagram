using QPP.Command;
using QPP.ComponentModel;
using QPP.Metadata;
using QPP.Runtime;
using QPP.Runtime.Serialization;
using System;
using System.ComponentModel;

namespace QPP.Modularity
{
    /// <summary>
    /// 呈現器,每個視圖都由一個呈現器控制
    /// </summary>
    public interface IPresenter : ILoadableItem
    {
        /// <summary>
        /// 視圖源對象，視圖加載后才賦值
        /// </summary>
        object Source { get; }
        /// <summary>
        /// 命令上下文
        /// </summary>
        ICommandContext CommandContext { get; }
        /// <summary>
        /// 加載事件，視圖加載時觸發
        /// </summary>
        event EventHandler Loaded;
        /// <summary>
        /// 關閉時事件，視圖關閉時觸發
        /// </summary>
        event CancelEventHandler Closing;
        /// <summary>
        /// 關閉后事件，視圖關閉后觸發
        /// </summary>
        event EventHandler Closed;
        /// <summary>
        /// 關閉時
        /// </summary>
        /// <param name="e"></param>
        void OnClosing(CancelEventArgs e);
        /// <summary>
        /// 關閉后
        /// </summary>
        /// <param name="e"></param>
        void OnClosed(EventArgs e);
        /// <summary>
        /// 元數據
        /// </summary>
        PresenterMetadata Metadata { get; }
        /// <summary>
        /// 設置參數
        /// </summary>
        void SetArgs(DataArgs args);
    }
}
