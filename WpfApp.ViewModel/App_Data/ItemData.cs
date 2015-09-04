using QPP.ComponentModel;
using System;

/*
 * 用于测试
 * 模拟树状结构的数据
 * 2015/7/16
 */
namespace WpfApp.ViewModel.App_Data
{
    [Serializable]
    public class ItemData : DataModel
    {
        public string ItemId
        {
            get { return Get<string>("ItemId"); }
            set { Set("ItemId", value); }
        }
        public string ItemParentId
        {
            get { return Get<string>("ItemParentId"); }
            set { Set("ItemParentId", value); }
        }
        public string Text
        {
            get { return Get<string>("Text"); }
            set { Set("Text", value); }
        }
        public string Desc
        {
            get { return Get<string>("Desc"); }
            set { Set("Desc", value); }
        }
        #region Left Top
        // 不是从原数据表中读取，是需要关联到其它保存节点样式的表中读取的
        public double Left
        {
            get { return Get<double>("Left"); }
            set { Set("Left", value); }
        }

        public double Top
        {
            get { return Get<double>("Top"); }
            set { Set("Top", value); }
        }

        public string ItemStyle
        {
            get { return Get<string>("ItemStyle"); }
            set { Set("ItemStyle", value); }
        }

        #endregion
        public ItemData() { }
        public ItemData(string id)
        {
            ItemId = id;
        }
        public ItemData(
            string id,
            string parentId,
            string text,
            string desc,
            string itemStyle)
        {
            ItemId = id;
            ItemParentId = parentId;
            Text = text;
            Desc = desc;
            ItemStyle = itemStyle;
        }
        public ItemData(
            string id,
            string parentId,
            string text,
            string desc,
            string itemStyle,
            double left,
            double top)
        {
            ItemId = id;
            ItemParentId = parentId;
            Text = text;
            Desc = desc;
            Left = left;
            Top = top;
            ItemStyle = itemStyle;
        }
    }


}
