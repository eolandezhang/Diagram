using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Command
{
    /// <summary>
    /// 指定框架提供的默認命令
    /// </summary>
    public enum RelayCommands
    {
        /// <summary>
        /// 創建
        /// </summary>
        Create,
        /// <summary>
        /// 查詢
        /// </summary>
        Search,
        /// <summary>
        /// 複製（表單）
        /// </summary>
        Copy,
        /// <summary>
        /// 編輯
        /// </summary>
        Edit,
        /// <summary>
        /// 刪除
        /// </summary>
        Delete,
        /// <summary>
        /// 批量刪除選中項目
        /// </summary>
        DeleteSelected,
        /// <summary>
        /// 保存
        /// </summary>
        Save,
        /// <summary>
        /// 導出
        /// </summary>
        Export,
        /// <summary>
        /// 導入
        /// </summary>
        Import,
        /// <summary>
        /// 批核
        /// </summary>
        Approve,
        /// <summary>
        /// 解批
        /// </summary>
        Disapprove,
        /// <summary>
        /// 撤銷
        /// </summary>
        Undo,
        /// <summary>
        /// 重做
        /// </summary>
        Redo,
        /// <summary>
        /// 重置過濾條件
        /// </summary>
        ResetFilter,
        /// <summary>
        /// 打開列表查詢
        /// </summary>
        OpenSearch,
        /// <summary>
        /// 瀏覽上一個
        /// </summary>
        BrowseBack,
        /// <summary>
        /// 瀏覽下一個
        /// </summary>
        BrowseForward,
        /// <summary>
        /// 首頁
        /// </summary>
        FirstPage,
        /// <summary>
        /// 尾頁
        /// </summary>
        LastPage,
        /// <summary>
        /// 下一頁
        /// </summary>
        NextPage,
        /// <summary>
        /// 上一頁
        /// </summary>
        PreviousPage,
        /// <summary>
        /// 跳到指定頁
        /// </summary>
        GoToPage,
        /// <summary>
        /// 刷新
        /// </summary>
        Refresh,
        /// <summary>
        /// 打印
        /// </summary>
        Print,
        /// <summary>
        /// 打印預覽
        /// </summary>
        PrintPreview,
    }
}
