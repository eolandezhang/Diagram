using QPP.Command;
using QPP.ComponentModel;
using QPP.Wpf.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Windows.MainMenu
{
    public class MainMenuItem : StatefulObject
    {
        public enum MainMenuGroup { Root, View, Tools, Help }

        public string Id
        {
            get { return Get<string>("Id"); }
            set { Set("Id", value); }
        }
        public string Name
        {
            get { return Get<string>("Name"); }
            set { Set("Name", value); }
        }
        public string ToolTip
        {
            get { return Get<string>("ToolTip"); }
            set { Set("ToolTip", value); }
        }
        public string Icon
        {
            get { return Get<string>("Icon"); }
            set { Set("Icon", value); }
        }
        public int VisibleIndex
        {
            get { return Get<int>("VisibleIndex"); }
            set { Set("VisibleIndex", value); }
        }
        public bool IsSeparator
        {
            get { return Get<bool>("IsSeparator"); }
            set { Set("IsSeparator", value); }
        }
        public MainMenuGroup Group
        {
            get { return Get<MainMenuGroup>("Group"); }
            set { Set("Group", value); }
        }
        public ICommand Command
        {
            get { return Get<ICommand>("Command"); }
            set { Set("Command", value); }
        }
        public object CommandParameter
        {
            get { return Get<object>("CommandParameter"); }
            set { Set("CommandParameter", value); }
        }
        public IList<TreeNode> Nodes { get; set; }
    }
}
