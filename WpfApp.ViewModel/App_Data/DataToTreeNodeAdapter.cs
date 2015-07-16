﻿using QPP.Wpf.ComponentModel;
/* ============================================================================== 
* 类名称：DataToTreeNodeAdapter 
* 类描述： 
* 创建人：eolandecheung 
* 创建时间：2015/7/16 17:25:14 
* 修改人： 
* 修改时间： 
* 修改备注： 
* @version 1.0 
* ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using QPP;
using QPP.Collections;
using QPP.Wpf.UI.TreeEditor;

namespace WpfApp.ViewModel.App_Data
{
    public class DataToTreeNodeAdapter : IDataToTreeNodeAdapter<ItemData>
    {
        public static DataToTreeNodeAdapter Default;

        static DataToTreeNodeAdapter()
        {
            Default = new DataToTreeNodeAdapter();
        }
        public ObservableCollection<TreeItemNode> CreateNodes(List<ItemData> itemDatas)
        {
            var result = new ObservableCollection<TreeItemNode>();
            var list = from m in itemDatas where m.ItemParentId.IsNullOrEmpty() select CreateNode(m, itemDatas);
            foreach (var treeNode in list)
            {
                result.Add(treeNode);
            }
            return result;
        }

        private TreeItemNode CreateNode(ItemData itemData, List<ItemData> itemDatas)
        {
            var node = new TreeItemNode { Id = itemData.ItemId/*id*/, Text = itemData.Text, Tag = itemData/*原始数据*/ };
            foreach (var child in itemDatas
                .Where(p => p.ItemParentId/*父id*/ == itemData.ItemId/*id*/)
                .Select(c => CreateNode(c, itemDatas)))
            {
                child.Parent = node;
                node.Children.Add(child);
            }
            return node;
        }
    }
}