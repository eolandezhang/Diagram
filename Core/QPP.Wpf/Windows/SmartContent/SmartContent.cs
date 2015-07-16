using QPP.ComponentModel;
using QPP.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace QPP.Wpf.Windows.SmartContent
{
    /// <summary>
    /// Content to show in SmartWindow
    /// </summary>
    public class SmartContent : StatefulObject
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
        /// 圖片
        /// </summary>
        public string ImageUrl
        {
            get { return Get<string>("ImageUrl"); }
            set { Set("ImageUrl", value); }
        }
        /// <summary>
        /// 內容
        /// </summary>
        public string ContentUri
        {
            get { return Get<string>("ContentUri"); }
            set { Set("ContentUri", value); }
        }
        /// <summary>
        /// 顯示順序
        /// </summary>
        public int VisibleIndex
        {
            get { return Get<int>("VisibleIndex"); }
            set { Set("VisibleIndex", value); }
        }
        /// <summary>
        /// 是否引人注目
        /// </summary>
        public bool IsStriking
        {
            get { return Get<bool>("IsStriking"); }
            set { Set("IsStriking", value); }
        }
    }
}
