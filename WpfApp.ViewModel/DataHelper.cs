using QPP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using WpfApp.ViewModel.App_Data;

namespace WpfApp.ViewModel
{
    public static class CopyOrPasteType
    {
        public static string None = "None";
        public static string Copy = "Copy";
        public static string Cut = "Cut";
    }
    public class DataHelper
    {
        public static DataHelper Default = new DataHelper();


        public void Delete(ObservableCollection<ItemData> selectedItems, RangeObservableCollection<ItemData> itemsSource)
        {
            if (selectedItems == null) return;
            var list = selectedItems.ToList();

            foreach (var selectedItem in list)
            {
                var all = GetAllSubItemDatas(itemsSource, selectedItem);
                all.Add(selectedItem);

                foreach (var itemData in all)
                {
                    itemsSource.Remove(itemData);
                }

            }
        }
        public void AddSiblingAction(ObservableCollection<ItemData> selectedItems, RangeObservableCollection<ItemData> itemsSource)
        {

            var selectedItem = GetSelectedItem(selectedItems);
            if (selectedItem != null)
            {
                if (selectedItem.ItemParentId.IsNullOrEmpty())
                {
                    AddAfterAction(selectedItems, itemsSource);
                    return;
                }
                var newItem = AddNew(selectedItem.ItemParentId, itemsSource);
                itemsSource.Add(newItem);
            }
        }
        public void AddAfterAction(ObservableCollection<ItemData> selectedItems, RangeObservableCollection<ItemData> itemsSource)
        {

            var selectedItem = GetSelectedItem(selectedItems);
            if (selectedItem == null) return;
            var newItem = AddNew(selectedItem.ItemId, itemsSource);
            if (newItem != null) itemsSource.Add(newItem);
        }
        public ItemData AddNew(string pid, double left, double top, RangeObservableCollection<ItemData> itemsSource)
        {
            return new ItemData(Guid.NewGuid().ToString(), pid, "Item " + itemsSource.Count, "", left, top, "");
        }
        public ItemData AddNew(string pid, RangeObservableCollection<ItemData> itemsSource)
        {
            if (pid == null) return null;
            return new ItemData(Guid.NewGuid().ToString(), pid, "Item" + itemsSource.Count(), "");
        }

        public ItemData GetSelectedItem(ObservableCollection<ItemData> selectedItems)
        {
            if (selectedItems.Count == 1)
            {
                var item = selectedItems.FirstOrDefault();
                var selectedItem = item;
                return selectedItem;
            }
            return null;
        }

        public void AddRootAction(RangeObservableCollection<ItemData> itemsSource)
        {
            if (itemsSource == null || !itemsSource.Any())
            {
                var newItem = AddNew("", 0, 0, itemsSource);
                itemsSource.Add(newItem);
            }
        }

        public void Copy(ObservableCollection<ItemData> selectedItems)
        {
            var list = selectedItems.ToList();
            if (!list.Any()) return;
            Clipboard.Clear();
            Clipboard.SetDataObject(list, false);
        }
        public void Cut(ObservableCollection<ItemData> selectedItems, RangeObservableCollection<ItemData> itemsSource)
        {
            Clipboard.Clear();
            var list = selectedItems.ToList();
            Clipboard.SetDataObject(list, false);
            if (selectedItems == null) return;
            foreach (var selectedItem in list)
            {
                itemsSource.Remove(selectedItem);
            }
        }
        public void Paste(List<ItemData> parentItemDatas, List<ItemData> copyedItemDatas, string type, RangeObservableCollection<ItemData> itemsSource, Point clickPoint)
        {
            var itemSource =
                  itemsSource.Select(
                      d => new ItemData(d.ItemId, d.ItemParentId, d.Text, d.Desc, d.Left, d.Top, d.ImageUri)).ToList();
            foreach (var parentItemData in parentItemDatas)
            {
                if (parentItemData == null) continue;
                var copys = GetItemDatasToBeCopy(copyedItemDatas, parentItemData, itemSource, type);
                PasteItemDatas(copys, parentItemData, itemsSource, clickPoint);
            }
        }
        List<ItemData> GetItemDatasToBeCopy(List<ItemData> dataItemOnClipBoard, ItemData parentItemData, List<ItemData> itemSource, string type)
        {
            var copys = new List<ItemData>();
            var copyItems = CopyedItems(dataItemOnClipBoard, itemSource, type);
            foreach (var copyItem in copyItems)
            {
                if (copyItem.ItemParentId == string.Empty)
                {
                    copyItem.ItemParentId = parentItemData.ItemId;
                }
                copys.Add(copyItem);
            }
            return copys;
        }
        void PasteItemDatas(List<ItemData> copys, ItemData selectedItem, RangeObservableCollection<ItemData> itemsSource, Point clickPoint)
        {
            var list = new List<ItemData>();
            var item = selectedItem;
            var roots = copys.Where(x => x.ItemParentId == item.ItemId);
            foreach (var itemData in roots)
            {
                if (itemData.ItemParentId.IsNullOrEmpty())
                {
                    itemData.Left = clickPoint.X;
                    itemData.Top = clickPoint.Y;
                }
                list.Add(itemData);
                list.AddRange(AddChild(copys, itemData));
            }
            itemsSource.AddRange(list);
        }
        List<ItemData> AddChild(List<ItemData> itemDatas, ItemData itemData)
        {
            var list = new List<ItemData>();
            var roots = itemDatas.Where(x => x.ItemParentId == itemData.ItemId);
            foreach (var data in roots)
            {
                list.Add(data);
                list.AddRange(AddChild(itemDatas, data));
            }
            return list;
        }
        List<ItemData> CopyedItems(List<ItemData> selectedItemDatas, List<ItemData> itemSource, string type)
        {
            var list = GetAllCopyItem(selectedItemDatas, itemSource);
            if (type == CopyOrPasteType.Copy)
            {
                var id = new Dictionary<string, string>();
                list.ForEach(itemData => { id[itemData.ItemId] = Guid.NewGuid().ToString("N"); });
                foreach (var itemData in list)
                {
                    itemData.ItemId = id[itemData.ItemId];
                    itemData.ItemParentId = id.ContainsKey(itemData.ItemParentId)
                        ? id[itemData.ItemParentId]
                        : string.Empty;
                }
            }
            else if (type == CopyOrPasteType.Cut)
            {
                var u = list.Where(x => list.All(y => y.ItemId != x.ItemParentId)).ToList();
                u.ForEach(x => x.ItemParentId = string.Empty);
                type = CopyOrPasteType.None;
            }
            return list;
        }
        List<ItemData> GetAllCopyItem(List<ItemData> selectedItemDatas, List<ItemData> itemSource)
        {

            var list = selectedItemDatas.Select(d => new ItemData(d.ItemId, d.ItemParentId, d.Text, d.Desc, d.Left, d.Top, d.ImageUri)).ToList();

            var childrens = new List<ItemData>();
            //把子节点也添加进来
            foreach (var selectedItemData in list)
            {
                var children = GetAllSubItemDatas(itemSource, selectedItemData);
                foreach (var d in children)
                {
                    if (childrens.Any(x => x.ItemId == d.ItemId)) continue;
                    var data = new ItemData(d.ItemId, d.ItemParentId, d.Text, d.Desc, d.Left, d.Top, d.ImageUri);
                    childrens.Add(data);
                }
            }
            var result = childrens.Where(children => selectedItemDatas.All(x => x.ItemId != children.ItemId)).ToList();
            result.AddRange(list);
            return result;
        }
        List<ItemData> GetAllSubItemDatas(IList<ItemData> itemDatas, ItemData itemData)
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
