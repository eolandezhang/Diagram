using QPP.Command;
using QPP.ComponentModel;
using QPP.Modularity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QPP.Layout
{
    /// <summary>
    /// 佈局管理
    /// </summary>
    public interface ILayoutManager
    {
        /// <summary>
        /// 活動內容變更
        /// </summary>
        event EventHandler<ContentEventArgs> ActivedContentChanged;
        /// <summary>
        /// 內容枚舉
        /// </summary>
        IEnumerable<IDockingContent> Contents { get; }

        /// <summary>
        /// 關閉指定內容
        /// </summary>
        /// <param name="content"></param>
        void Close(IDockingContent content);

        /// <summary>
        /// 關閉當前內容
        /// </summary>
        void CloseCurrent();

        /// <summary>
        /// 是否能關閉當前內容，當前內容不為空且內容可以被關閉時為true，否則為false
        /// </summary>
        /// <returns></returns>
        bool CanCloseCurrent();

        /// <summary>
        /// 關閉所有內容
        /// </summary>
        void CloseAll();

        /// <summary>
        /// 是否能關閉所有內容，當有可以被關閉的內容時為true，否則為false
        /// </summary>
        /// <returns></returns>
        bool CanCloseAll();

        void Show(IDockingContent content);

        /// <summary>
        /// 佈局序列化
        /// </summary>
        /// <returns>序列化結果</returns>
        string LayoutSerialize();

        /// <summary>
        /// 佈局反系列化
        /// </summary>
        /// <param name="layout">序列化結果</param>
        void LayoutDeserialize(string layout);

    }
}
