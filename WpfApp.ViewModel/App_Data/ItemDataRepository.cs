using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfApp.ViewModel.App_Data
{
    public class ItemDataRepository
    {
        public static ItemDataRepository Default;
        static ItemDataRepository()
        {
            Default = new ItemDataRepository();
        }

        public ItemData AddNew(string pid, double left, double top)
        {
            return new ItemData(Guid.NewGuid().ToString(), pid, "Item", "", left, top, "");
        }
        public List<ItemData> GetAllSubItemDatas(IList<ItemData> itemDatas, ItemData itemData)
        {
            var result = new List<ItemData>();
            var child = new List<ItemData>();
            var list = itemDatas.Where(x => x.ItemParentId == itemData.ItemId);
            foreach (var subItem in list.Where(subitem => !result.Contains(subitem)))
            {
                child.Add(subItem);
                result.Add(subItem);
                foreach (var item in child)
                {
                    result.AddRange(GetAllSubItemDatas(itemDatas, item));
                }
            }
            return result;
        }
    }
}
