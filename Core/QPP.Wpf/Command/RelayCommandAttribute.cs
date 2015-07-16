using QPP.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace QPP.Wpf.Command
{
    /// <summary>
    /// 命令描述
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RelayCommandAttribute : Attribute
    {
        /// <summary>
        /// 命令的用法
        /// </summary>
        public CommandUsage Usage { get; set; }
        /// <summary>
        /// 快捷鍵,對於CommandUsage.KeyBinding有效
        /// </summary>
        public Key Key { get; set; }
        /// <summary>
        /// 修改键,對於CommandUsage.KeyBinding有效
        /// </summary>
        public ModifierKeys ModifierKeys { get; set; }
        /// <summary>
        /// 目標
        /// </summary>
        public string Target { get; set; }
        /// <summary>
        /// 圖標
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 顯示順序
        /// </summary>
        public int VisibleIndex { get; set; }
        /// <summary>
        /// 是否使用Separator開始一個新的組
        /// </summary>
        public bool BeginGroup { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public RelayCommandAttribute() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">名稱</param>
        public RelayCommandAttribute(string name)
        {
            Name = name;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">名稱</param>
        /// <param name="usage">用法</param>
        public RelayCommandAttribute(string name, CommandUsage usage)
        {
            Name = name;
            Usage = usage;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseOn">命令描述</param>
        public RelayCommandAttribute(RelayCommandAttribute baseOn)
        {
            Name = baseOn.Name;
            Usage = baseOn.Usage;
            Target = baseOn.Target;
            Icon = baseOn.Icon;
            Key = baseOn.Key;
            ModifierKeys = baseOn.ModifierKeys;
            BeginGroup = baseOn.BeginGroup;
            VisibleIndex = baseOn.VisibleIndex;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseOn">命令原型</param>
        public RelayCommandAttribute(RelayCommands baseOn)
            : this(GetCommand(baseOn))
        {
        }

        static RelayCommandAttribute GetCommand(RelayCommands cmd)
        {
            switch (cmd)
            {
                case RelayCommands.Create: return CreateCommand;
                case RelayCommands.Search: return SearchCommand;
                case RelayCommands.Copy: return CopyCommand;
                case RelayCommands.Edit: return EditCommand;
                case RelayCommands.Delete: return DeleteCommand;
                case RelayCommands.DeleteSelected: return DeleteSelectedCommand;
                case RelayCommands.Save: return SaveCommand;
                case RelayCommands.Export: return ExportCommand;
                case RelayCommands.Import: return ImportCommand;
                case RelayCommands.Approve: return ApproveCommand;
                case RelayCommands.Disapprove: return DisapproveCommand;
                case RelayCommands.Undo: return UndoCommand;
                case RelayCommands.Redo: return RedoCommand;
                case RelayCommands.ResetFilter: return ResetFilterCommand;
                case RelayCommands.OpenSearch: return OpenSearchCommand;
                case RelayCommands.BrowseBack: return BrowseBackCommand;
                case RelayCommands.BrowseForward: return BrowseForwardCommand;
                case RelayCommands.FirstPage: return FirstPageCommand;
                case RelayCommands.LastPage: return LastPageCommand;
                case RelayCommands.NextPage: return NextPageCommand;
                case RelayCommands.PreviousPage: return PreviousPageCommand;
                case RelayCommands.GoToPage: return GoToPageCommand;
                case RelayCommands.Refresh: return RefreshCommand;
                case RelayCommands.Print: return PrintCommand;
                case RelayCommands.PrintPreview: return PrintPreviewCommand;
            }
            return null;
        }

        static RelayCommandAttribute()
        {
            //SearchCommand
            SearchCommand = new RelayCommandAttribute("SearchCommand", CommandUsage.ToolBar | CommandUsage.KeyBinding);
            SearchCommand.VisibleIndex = 10;
            SearchCommand.Icon = "/QPP.Resources;component/Images/page_find.png";
            SearchCommand.Key = Key.Q;
            SearchCommand.ModifierKeys = ModifierKeys.Control;
            //ResetFilterCommand
            ResetFilterCommand = new RelayCommandAttribute("ResetFilterCommand", CommandUsage.ToolBar);
            ResetFilterCommand.VisibleIndex = 20;
            ResetFilterCommand.Icon = "/QPP.Resources;component/Images/textfield_rename.png";
            //CreateCommand
            CreateCommand = new RelayCommandAttribute("CreateCommand", CommandUsage.ToolBar | CommandUsage.KeyBinding);
            CreateCommand.VisibleIndex = 30;
            CreateCommand.Icon = "/QPP.Resources;component/Images/page_add.png";
            CreateCommand.BeginGroup = true;
            CreateCommand.Key = Key.N;
            CreateCommand.ModifierKeys = ModifierKeys.Control;
            //DeleteSelectedCommand
            DeleteSelectedCommand = new RelayCommandAttribute("DeleteSelectedCommand", CommandUsage.ToolBar | CommandUsage.KeyBinding);
            DeleteSelectedCommand.VisibleIndex = 40;
            DeleteSelectedCommand.Icon = "/QPP.Resources;component/Images/page_delete.png";
            DeleteSelectedCommand.Key = Key.Delete;
            DeleteSelectedCommand.ModifierKeys = ModifierKeys.Control;
            //ExportCommand
            ExportCommand = new RelayCommandAttribute("ExportCommand", CommandUsage.ToolBar);
            ExportCommand.VisibleIndex = 50;
            ExportCommand.Icon = "/QPP.Resources;component/Images/application_put.png";
            ImportCommand = new RelayCommandAttribute("ImportCommand", CommandUsage.ToolBar);
            ImportCommand.VisibleIndex = 60;
            //CopyCommand
            CopyCommand = new RelayCommandAttribute("CopyCommand", CommandUsage.ToolBar | CommandUsage.KeyBinding);
            CopyCommand.VisibleIndex = 70;
            CopyCommand.Icon = "/QPP.Resources;component/Images/page_copy.png";
            CopyCommand.BeginGroup = true;
            CopyCommand.Key = Key.D;
            CopyCommand.ModifierKeys = ModifierKeys.Control;
            //SaveCommand
            SaveCommand = new RelayCommandAttribute("SaveCommand", CommandUsage.ToolBar | CommandUsage.KeyBinding);
            SaveCommand.VisibleIndex = 80;
            SaveCommand.Icon = "/QPP.Resources;component/Images/save.png";
            SaveCommand.BeginGroup = false;
            SaveCommand.Key = Key.S;
            SaveCommand.ModifierKeys = ModifierKeys.Control;
            //UndoCommand
            UndoCommand = new RelayCommandAttribute("UndoCommand", CommandUsage.ToolBar | CommandUsage.KeyBinding);
            UndoCommand.VisibleIndex = 90;
            UndoCommand.Icon = "/QPP.Resources;component/Images/undo.png";
            UndoCommand.BeginGroup = true;
            UndoCommand.Key = Key.Z;
            UndoCommand.ModifierKeys = ModifierKeys.Control;
            //RedoCommand
            RedoCommand = new RelayCommandAttribute("RedoCommand", CommandUsage.ToolBar | CommandUsage.KeyBinding);
            RedoCommand.VisibleIndex = 100;
            RedoCommand.Icon = "/QPP.Resources;component/Images/redo.png";
            RedoCommand.Key = Key.Y;
            RedoCommand.ModifierKeys = ModifierKeys.Control;
            //ApproveCommand
            ApproveCommand = new RelayCommandAttribute("ApproveCommand", CommandUsage.ToolBar);
            ApproveCommand.VisibleIndex = 110;
            ApproveCommand.Icon = "/QPP.Resources;component/Images/accept.png";
            //DisapproveCommand
            DisapproveCommand = new RelayCommandAttribute("DisapproveCommand", CommandUsage.ToolBar);
            DisapproveCommand.VisibleIndex = 120;
            DisapproveCommand.Icon = "/QPP.Resources;component/Images/cancel.png";
            //OpenSearchCommand
            OpenSearchCommand = new RelayCommandAttribute("OpenSearchCommand", CommandUsage.ToolBar);
            OpenSearchCommand.VisibleIndex = 140;
            OpenSearchCommand.Icon = "/QPP.Resources;component/Images/table_find.png";
            OpenSearchCommand.BeginGroup = true;
            //EditCommand
            EditCommand = new RelayCommandAttribute("EditCommand", CommandUsage.ContextMenu);
            //DeleteCommand
            DeleteCommand = new RelayCommandAttribute("DeleteCommand", CommandUsage.ContextMenu);
            //BrowseBackCommand
            BrowseBackCommand = new RelayCommandAttribute("BrowseBackCommand", CommandUsage.KeyBinding);
            BrowseBackCommand.Key = Key.Left;
            BrowseBackCommand.ModifierKeys = ModifierKeys.Alt;
            //BrowseForwardCommand
            BrowseForwardCommand = new RelayCommandAttribute("BrowseForwardCommand", CommandUsage.KeyBinding);
            BrowseForwardCommand.Key = Key.Right;
            BrowseForwardCommand.ModifierKeys = ModifierKeys.Alt;
            //FirstPageCommand
            FirstPageCommand = new RelayCommandAttribute("FirstPageCommand", CommandUsage.KeyBinding);
            FirstPageCommand.Key = Key.Home;
            FirstPageCommand.ModifierKeys = ModifierKeys.Alt;
            //LastPageCommand
            LastPageCommand = new RelayCommandAttribute("LastPageCommand", CommandUsage.KeyBinding);
            LastPageCommand.Key = Key.End;
            LastPageCommand.ModifierKeys = ModifierKeys.Alt;
            //NextPageCommand
            NextPageCommand = new RelayCommandAttribute("NextPageCommand", CommandUsage.KeyBinding);
            NextPageCommand.Key = Key.PageDown;
            NextPageCommand.ModifierKeys = ModifierKeys.Alt;
            //PreviousPageCommand
            PreviousPageCommand = new RelayCommandAttribute("PreviousPageCommand", CommandUsage.KeyBinding);
            PreviousPageCommand.Key = Key.PageUp;
            PreviousPageCommand.ModifierKeys = ModifierKeys.Alt;
            //GoToPageCommand
            GoToPageCommand = new RelayCommandAttribute("GoToPageCommand", CommandUsage.None);
            //RefreshCommand
            RefreshCommand = new RelayCommandAttribute("RefreshCommand", CommandUsage.ToolBar | CommandUsage.KeyBinding);
            RefreshCommand.VisibleIndex = 130;
            RefreshCommand.Icon = "/QPP.Resources;component/Images/refresh.png";
            RefreshCommand.Key = Key.F5;
            RefreshCommand.ModifierKeys = ModifierKeys.Control;
            //PrintCommand
            PrintCommand = new RelayCommandAttribute("PrintCommand", CommandUsage.ToolBar);
            PrintCommand.VisibleIndex = 150;
            PrintCommand.Icon = "/QPP.Resources;component/Images/printer.png";
            //PrintPriviewCommand
            PrintPreviewCommand = new RelayCommandAttribute("PrintPreviewCommand", CommandUsage.ToolBar);
            PrintPreviewCommand.VisibleIndex = 160;
            PrintPreviewCommand.Icon = "/QPP.Resources;component/Images/page_print.png";
        }

        /// <summary>
        /// 單據創建(Ctrl+N)
        /// </summary>
        public static RelayCommandAttribute CreateCommand;
        /// <summary>
        /// 查詢創建(Ctrl+Q)
        /// </summary>
        public static RelayCommandAttribute SearchCommand;
        /// <summary>
        /// 單據複製(Ctrl+D)
        /// </summary>
        public static RelayCommandAttribute CopyCommand;
        /// <summary>
        /// 單據編輯
        /// </summary>
        public static RelayCommandAttribute EditCommand;
        /// <summary>
        /// 單據刪除(Ctrl+Delete)
        /// </summary>
        public static RelayCommandAttribute DeleteCommand;
        /// <summary>
        /// 批量單據刪除
        /// </summary>
        public static RelayCommandAttribute DeleteSelectedCommand;
        /// <summary>
        /// 單據保存(Ctrl+S)
        /// </summary>
        public static RelayCommandAttribute SaveCommand;
        /// <summary>
        /// 數據導出
        /// </summary>
        public static RelayCommandAttribute ExportCommand;
        /// <summary>
        /// 單據導入
        /// </summary>
        public static RelayCommandAttribute ImportCommand;
        /// <summary>
        /// 批核
        /// </summary>
        public static RelayCommandAttribute ApproveCommand;
        /// <summary>
        /// 解除批核
        /// </summary>
        public static RelayCommandAttribute DisapproveCommand;
        /// <summary>
        /// 撤銷(Ctrl+Z)
        /// </summary>
        public static RelayCommandAttribute UndoCommand;
        /// <summary>
        /// 恢復(Ctrl+Y)
        /// </summary>
        public static RelayCommandAttribute RedoCommand;
        /// <summary>
        /// 重置查詢條件
        /// </summary>
        public static RelayCommandAttribute ResetFilterCommand;
        /// <summary>
        /// 打開查詢頁面
        /// </summary>
        public static RelayCommandAttribute OpenSearchCommand;
        /// <summary>
        /// 瀏覽上一個(Alt+Left)
        /// </summary>
        public static RelayCommandAttribute BrowseBackCommand;
        /// <summary>
        /// 瀏覽下一個(Alt+Right)
        /// </summary>
        public static RelayCommandAttribute BrowseForwardCommand;
        /// <summary>
        /// 第一頁(Alt+Home)
        /// </summary>
        public static RelayCommandAttribute FirstPageCommand;
        /// <summary>
        /// 最后一頁(Alt+End)
        /// </summary>
        public static RelayCommandAttribute LastPageCommand;
        /// <summary>
        /// 下一頁(Alt+PageDown)
        /// </summary>
        public static RelayCommandAttribute NextPageCommand;
        /// <summary>
        /// 上一頁(Alt+PageUp)
        /// </summary>
        public static RelayCommandAttribute PreviousPageCommand;
        /// <summary>
        /// 跳到指定頁
        /// </summary>
        public static RelayCommandAttribute GoToPageCommand;
        /// <summary>
        /// 刷新(Ctrl+F5)
        /// </summary>
        public static RelayCommandAttribute RefreshCommand;
        /// <summary>
        /// 打印
        /// </summary>
        public static RelayCommandAttribute PrintCommand;
        /// <summary>
        /// 打印預覽
        /// </summary>
        public static RelayCommandAttribute PrintPreviewCommand;
    }
}
