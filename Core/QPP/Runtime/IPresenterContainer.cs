using QPP.ComponentModel;
using QPP.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Runtime
{
    /// <summary>
    /// 呈現器容器
    /// </summary>
    public interface IPresenterContainer
    {
        /// <summary>
        /// 活動呈現器變更
        /// </summary>
        event EventHandler<PresenterEventArgs> ActivedPresenterChanged;
        /// <summary>
        /// 呈現器關閉后
        /// </summary>
        event EventHandler<PresenterOpeningEventArgs> PresenterOpening;
        /// <summary>
        /// 呈現器初始化
        /// </summary>
        event EventHandler<PresenterEventArgs> PresenterInitialized;
        /// <summary>
        /// 呈現器加載
        /// </summary>
        event EventHandler<PresenterEventArgs> PresenterLoaded;
        /// <summary>
        /// 呈現器關閉時
        /// </summary>
        event EventHandler<PresenterClosingEventArgs> PresenterClosing;
        /// <summary>
        /// 呈現器關閉后
        /// </summary>
        event EventHandler<PresenterEventArgs> PresenterClosed;
        /// <summary>
        /// 當前活動的呈現器
        /// </summary>
        IPresenter ActivedPresenter { get; }
        /// <summary>
        /// 列舉呈現器
        /// </summary>
        IEnumerable<IPresenter> Presenters { get; }
        /// <summary>
        /// 打開對話框，返回對話框的呈現器，新窗口關閉后才返回
        /// </summary>
        IPresenter OpenDialog(object owner, string uri, DataArgs args = null);
        /// <summary>
        /// 打開視圖，返回視圖的呈現器
        /// <para/>
        /// 當key存在時，直接顯示內容，如果不存在，根據uri加載內容。
        /// 如果加載的內容是IDockingContent類型，使用佈局管理器ILyaoutManager打開，
        /// 如果加載的內容是Window, 直接顯示窗口。<para/>
        /// 當key為空時，嘗試使用uri參數contentKey的值作為contentKey。
        /// 如: //QPP.Master.Wpf;component/User/UserContent.xaml?contentKey=theKey。
        /// 當沒有key參數，或者為空時，使用uri作為Key。
        /// </summary>
        IPresenter Open(string uri, DataArgs args = null, string key = null);
        /// <summary>
        /// 根據呈現器顯示視圖
        /// </summary>
        void Show(IPresenter presenter);
        /// <summary>
        /// 根據呈現器關閉視圖
        /// </summary>
        void Close(IPresenter presenter);
        /// <summary>
        /// 是否包含指定Key的呈現器
        /// </summary>
        bool ContainsPresenter(string key);
        /// <summary>
        /// 根據Key獲取呈現器
        /// </summary>
        IPresenter GetPresenter(string key);
        /// <summary>
        /// 獲取視圖對象(Window或者IDockingContent...)
        /// </summary>
        IView GetView(IPresenter presenter);
    }
}
