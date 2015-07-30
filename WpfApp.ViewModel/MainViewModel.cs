using System;
using System.Collections;
using QPP;
using QPP.Command;
using QPP.ComponentModel;
using QPP.Wpf.Command;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using QPP.Wpf.UI.TreeEditor;
using WpfApp.ViewModel.App_Data;

namespace WpfApp.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public string Title { get { return Get<string>("Title"); } set { Set("Title", value); } }
        public bool SingleRoot { get { return Get<bool>("SingleRoot"); } set { Set("SingleRoot", value); } }
        public ObservableCollection<ItemData> ItemsSource { get; set; }
        public ObservableCollection<ItemData> SelectedItems { get; set; }
        public ObservableCollection<ItemData> DeletedItems { get; set; }
        public ItemData SelectedItem { get { return Get<ItemData>("SelectedItem"); } set { Set("SelectedItem", value); } }
        public MainViewModel()
        {
            SingleRoot = true;
            SelectedItems = new ObservableCollection<ItemData>();
            DeletedItems = new ObservableCollection<ItemData>();
            Title = "Tree Editor";
            LoadData();

            SelectedItems.CollectionChanged += (d, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    var item = SelectedItems.FirstOrDefault();
                    if (item != null)
                    {
                        var data = item;
                        if (data != null)
                        {
                            SelectedItem = data;
                            //MessageBox.Show(data.Text);
                        }
                    }

                }
            };
        }

        private void LoadData()
        {
            ItemsSource = ItemDataRepository.Default.DataCollection;
        }
        public Point ClickPoint
        {
            get
            {
                return Get<Point>("ClickPoint");
            }
            set
            {
                Set("ClickPoint", value);
            }
        }

        #region Commands
        public ICommand ReloadCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ItemsSource.Clear();
                    ItemsSource.Add(new ItemData("0", "", "0.0", "Root　Item1", "Images/fix.png"));
                    ItemsSource.Add(new ItemData("1", "0", "1.1", "-", "Images/green.png"));
                    ItemsSource.Add(new ItemData("3", "1", "2.1", "-"));
                    ItemsSource.Add(new ItemData("5", "3", "2.2", "-"));
                    ItemsSource.Add(new ItemData("2", "0", "1.2", "-"));
                    ItemsSource.Add(new ItemData("4", "2", "3.1", "-"));
                    ItemsSource.Add(new ItemData("7", "4", "3.2", "-"));
                    ItemsSource.Add(new ItemData("6", "0", "1.3", "-"));
                    ItemsSource.Add(new ItemData(Guid.NewGuid().ToString(), "0", "Item 1", ""));
                    ItemsSource.Add(new ItemData(Guid.NewGuid().ToString(), "0", "Item 2", ""));
                    ItemsSource.Add(new ItemData(Guid.NewGuid().ToString(), "0", "Item 3", ""));

                });
            }
        }
        #region AddRootCommand

        public ICommand AddRootCommand
        {
            get
            {
                return new RelayCommand(AddRootAction, CanAddRoot);

            }
        }

        private bool CanAddRoot()
        {
            if (ItemsSource == null || !ItemsSource.Any()) { return true; }
            if (ItemsSource != null && ItemsSource.Count == 1 && SingleRoot)
            {
                return false;
            }
            return false;
        }

        private void AddRootAction()
        {
            if (ItemsSource == null || !ItemsSource.Any())
            {
                if (ItemsSource == null) ItemsSource = new ObservableCollection<ItemData>();
                var newItem = ItemDataRepository.Default.AddNew("");
                ItemsSource.Add(newItem);
            }
        }
        #endregion
        ItemData GetSelectedItem()
        {
            if (SelectedItems.Count == 1)
            {
                var item = SelectedItems.FirstOrDefault();
                var selectedItem = item;
                return selectedItem;
            }
            return null;
        }
        #region AddAfterCommand
        public ICommand AddAfterCommand { get { return new RelayCommand(AddAfterAction, () => SelectedItems.Count == 1); } }

        void AddAfterAction()
        {
            var selectedItem = GetSelectedItem();
            if (selectedItem != null)
            {
                var newItem = ItemDataRepository.Default.AddNew(selectedItem.ItemId);
                ItemsSource.Add(newItem);
            }
        }
        #endregion
        //public bool EnableCommand()
        //{
        //    var selectedItem = GetSelectedItem();
        //    if (selectedItem != null)
        //    {
        //        //return !SelectedItem.DiagramControl.IsOnEditing;
        //        return true;
        //    }
        //    return false;
        //}

        #region AddSiblingCommand
        public ICommand AddSiblingCommand { get { return new RelayCommand(AddSiblingAction, () => SelectedItems.Count == 1); } }
        private void AddSiblingAction()
        {
            var selectedItem = GetSelectedItem();
            if (selectedItem != null)
            {
                if (selectedItem.ItemParentId.IsNullOrEmpty())
                {
                    AddAfterAction();
                    return;
                }
                var newItem = ItemDataRepository.Default.AddNew(selectedItem.ItemParentId);
                ItemsSource.Add(newItem);
            }
        }
        #endregion

        #region DeleteCommand
        public ICommand DeleteCommand { get { return new RelayCommand(DeleteAction, () => SelectedItems.Any()); } }
        private void DeleteAction()
        {
            if (SelectedItems == null) return;
            var list = SelectedItems.ToList();
            foreach (var selectedItem in list)
            {
                ItemsSource.Remove(selectedItem);
            }
        }
        #endregion

        #region CanvasDoubleClickCommand
        public ICommand CanvasDoubleClickCommand
        {
            get
            {
                return new RelayCommand<Point>((p) =>
                    {
                        if (ItemsSource != null && ItemsSource.Any())
                        {
                            if (!SingleRoot)
                            {
                                var newItem = ItemDataRepository.Default.AddNew("", p.X, p.Y);
                                ItemsSource.Add(newItem);
                            }
                        }
                    });
            }
        }
        #endregion

        #region ShowMessage
        public ICommand ShowMessageCommand
        {
            get { return new RelayCommand(() => { MessageBox.Show("Show Message."); }); }
        }
        #endregion

        #region Copy Cut Past Command
        public ICommand CopyCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Clipboard.Clear();
                    var list = SelectedItems.ToList();
                    Clipboard.SetDataObject(list, false);
                }, () => SelectedItems.Any());
            }
        }

        public ICommand CutCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Clipboard.Clear();
                    var cutItmes = SelectedItems.ToList();
                    var list = SelectedItems.ToList();
                    Clipboard.SetDataObject(list, false);
                    foreach (var itemData in cutItmes)
                    {
                        ItemsSource.Remove(itemData);
                    }
                }, () => SelectedItems.Any());
            }
        }

        public ICommand PasteCommand
        {
            get
            {
                return new RelayCommand(PasteAction, () => SelectedItems.Any());
            }
        }

        void PasteAction()
        {
            var dataOnClipBoard = Clipboard.GetDataObject();
            if (dataOnClipBoard == null) return;

            var pasteToItemDatas = SelectedItems.ToList();
            if (!pasteToItemDatas.Any()) { return; }
            var copyedItemDatas = dataOnClipBoard.GetData(typeof(List<ItemData>)) as List<ItemData>;//从剪切板中取得数据
            if (copyedItemDatas == null) { return; }

            Paste(pasteToItemDatas, copyedItemDatas);

        }

        void Paste(List<ItemData> parentItemDatas, List<ItemData> copyedItemDatas)
        {
            foreach (var parentItemData in parentItemDatas)
            {
                if (parentItemData == null) continue;
                var copys = GetItemDatasToBeCopy(copyedItemDatas, parentItemData);
                PasteItemDatas(copys, parentItemData);
            }
        }

        List<ItemData> GetItemDatasToBeCopy(List<ItemData> dataItemOnClipBoard, ItemData parentItemData)
        {
            var copys = new List<ItemData>();
            var copyItems = GetCopyedItems(dataItemOnClipBoard);
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

        void PasteItemDatas(List<ItemData> copys, ItemData selectedItem)
        {
            var item = selectedItem;
            var roots = copys.Where(x => x.ItemParentId == item.ItemId);
            foreach (var itemData in roots)
            {
                if (itemData.ItemParentId.IsNullOrEmpty())
                {
                    itemData.Left = ClickPoint.X;
                    itemData.Top = ClickPoint.Y;
                }
                ItemsSource.Add(itemData);
                AddChild(copys, itemData);
            }
        }

        void AddChild(List<ItemData> itemDatas, ItemData itemData)
        {
            var roots = itemDatas.Where(x => x.ItemParentId == itemData.ItemId);
            foreach (var data in roots)
            {
                ItemsSource.Add(data);
                AddChild(itemDatas, data);
            }
        }

        List<ItemData> GetAllCopyItem(List<ItemData> selectedItemDatas)
        {
            var list = selectedItemDatas.Select(d => new ItemData(d.ItemId, d.ItemParentId, d.Text, d.Desc, d.Left, d.Top, d.ImageUri)).ToList();

            var childrens = new List<ItemData>();
            //把子节点也添加进来
            foreach (var selectedItemData in selectedItemDatas)
            {
                var children = ItemDataRepository.Default.GetAllSubItemDatas(ItemsSource, selectedItemData);
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

        List<ItemData> GetCopyedItems(List<ItemData> selectedItemDatas)
        {
            var list = GetAllCopyItem(selectedItemDatas);
            var id = new Dictionary<string, string>();
            list.ForEach(itemData => { id[itemData.ItemId] = Guid.NewGuid().ToString("N"); });
            foreach (var itemData in list)
            {
                itemData.ItemId = id[itemData.ItemId];
                itemData.ItemParentId = id.ContainsKey(itemData.ItemParentId) ?
                    id[itemData.ItemParentId] : string.Empty;
            }
            return list;
        }

        #endregion
        #endregion
    }
}
