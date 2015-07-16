using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QPP.Wpf.UI.Models;
using QPP.ComponentModel;
using System.Collections.ObjectModel;

namespace QPP.Wpf.ComponentModel
{
    public class TreeNode : HierarchicalData<TreeNode>
    {
        public string Id
        {
            get { return Get<string>("Id"); }
            set { Set("Id", value); }
        }

        public object Tag
        {
            get { return Get<object>("Tag"); }
            set { Set("Tag", value); }
        }
    }
}
