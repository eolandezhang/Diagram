using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Command
{
    /// <summary>
    /// 命令的用法
    /// </summary>
    [Flags]
    public enum CommandUsage
    {
        /// <summary>
        /// 不作處理
        /// </summary>
        None = 0,
        /// <summary>
        /// 綁定到工具條
        /// </summary>
        ToolBar = 2,
        /// <summary>
        /// 綁定到右擊菜單
        /// </summary>
        ContextMenu = 4,
        /// <summary>
        /// 綁定到快捷鍵
        /// </summary>
        KeyBinding = 8,
    }
}
