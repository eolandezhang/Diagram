using QPP.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace QPP.Wpf.Windows.ToolBar
{
    /// <summary>
    /// 工具欄項目
    /// </summary>
    public class ToolBarItem : StatefulObject
    {
        /// <summary>
        /// 名稱
        /// </summary>
        public string Name
        {
            get { return Get<string>("Name"); }
            set { Set("Name", value); }
        }
        /// <summary>
        /// 提示
        /// </summary>
        public string ToolTip
        {
            get { return Get<string>("ToolTip"); }
            set { Set("ToolTip", value); }
        }
        /// <summary>
        /// 命令
        /// </summary>
        public ICommand Command
        {
            get { return Get<ICommand>("Command"); }
            set { Set("Command", value); }
        }
        /// <summary>
        /// 圖片
        /// </summary>
        public string ImageUrl
        {
            get { return Get<string>("ImageUrl"); }
            set { Set("ImageUrl", value); }
        }
        /// <summary>
        /// 命令的上下文對象
        /// </summary>
        public object DataContext
        {
            get { return Get<object>("DataContext"); }
            set { Set("DataContext", value); }
        }
        /// <summary>
        /// 顯示順序
        /// </summary>
        public int VisibleIndex
        {
            get { return Get<int>("VisibleIndex"); }
            set { Set("VisibleIndex", value); }
        }
    }
}
