/* ============================================================================== 
* 类名称：TreeItemNode 
* 类描述： 
* 创建人：eolandecheung 
* 创建时间：2015/7/16 18:24:46 
* 修改人： 
* 修改时间： 
* 修改备注： 
* @version 1.0 
* ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QPP.ComponentModel;

namespace QPP.Wpf.UI.TreeEditor
{
    public class TreeItemNode : HierarchicalData<TreeItemNode>
    {
        public string Id
        {
            get { return Get<string>("Id"); }
            set { Set("Id", value); }
        }

        public string Text
        {
            get { return Get<string>("Text"); }
            set { Set("Text", value); }
        }

        public object Tag
        {
            get { return Get<object>("Tag"); }
            set { Set("Tag", value); }
        }
    }
}
