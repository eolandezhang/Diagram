using QPP.ComponentModel;
using System;
/*
 * 用于测试
 * 模拟树状结构的数据
 * 2015/7/16
 */
namespace WpfApp.ViewModel.App_Data
{
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

        public ItemData(string id)
        {
            ItemId = id;
        }
        public ItemData(
            string id,
            string parentId,
            string text,
            string desc)
        {
            ItemId = id;
            ItemParentId = parentId;
            Text = text;
            Desc = desc;
        }
    }
}
