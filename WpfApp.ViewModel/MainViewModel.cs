using QPP;
using QPP.Command;
using QPP.ComponentModel;
using QPP.Wpf.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using WpfApp.ViewModel.App_Data;

namespace WpfApp.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public int Num { get { return Get<int>("Num"); } set { Set("Num", value); } }
        public string Title { get { return Get<string>("Title"); } set { Set("Title", value); } }
        public bool SingleRoot { get { return Get<bool>("SingleRoot"); } set { Set("SingleRoot", value); } }
        public RangeObservableCollection<ItemData> ItemsSource { get; set; }
        public ObservableCollection<ItemData> SelectedItems { get; set; }
        public ObservableCollection<ItemData> DeletedItems { get; set; }
        public ItemData SelectedItem { get { return Get<ItemData>("SelectedItem"); } set { Set("SelectedItem", value); } }
        public bool IsAddAfter { get { return Get<bool>("IsAddAfter"); } set { Set("IsAddAfter", value); } }
        public Point ClickPoint { get { return Get<Point>("ClickPoint"); } set { Set("ClickPoint", value); } }
        private string _type = CopyOrPasteType.None;
        public string Type { get { return _type; } set { _type = value; } }
        public MainViewModel()
        {
            Title = "Tree Editor";
            SingleRoot = false;
            SelectedItems = new ObservableCollection<ItemData>();
            DeletedItems = new ObservableCollection<ItemData>();
            ItemsSource = new RangeObservableCollection<ItemData>();
            SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
            //ReloadCommand.Execute(null);
        }

        private void SelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var item = SelectedItems.FirstOrDefault();
                if (item == null) return;
                var data = item;
                SelectedItem = data;
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
                    var list = new List<ItemData>()
                    {
                        new ItemData("0", "", "0.0", "Root　Item1","{'BorderBrush':'#FF87CEEB','Background':'#00FFFFFF'}", "Images/fix.png"),
                        new ItemData("1", "0", "1.1", "-","{'BorderBrush':'#FF87CEEB','Background':'#000000FF'}", "Images/green.png"),
                        new ItemData("3", "1", "2.1", "-","{'BorderBrush':'#FF87CEEB','Background':'#000000FF'}"),
                        new ItemData("5", "3", "2.2", "-","{'BorderBrush':'#FF87CEEB','Background':'#000000FF'}"),
                        new ItemData("2", "0", "1.2", "-","{'BorderBrush':'#FF87CEEB','Background':'#000000FF'}"),
                        new ItemData("4", "2", "3.1", "-","{'BorderBrush':'#FF87CEEB','Background':'#000000FF'}"),
                        new ItemData("7", "4", "3.2", "-","{'BorderBrush':'#FF87CEEB','Background':'#000000FF'}"),
                        new ItemData("6", "0", "1.3", "-","{'BorderBrush':'#FFFF0000','Background':'#000000FF'}")
                    };
                    for (var i = 0; i < Num; i++)
                    {
                        list.Add(new ItemData(Guid.NewGuid().ToString(), "0", "Item " + i, "{'BorderBrush':'#FF87CEEB','Background':'#000000FF'}", "Images/green.png"));
                    }
                    ItemsSource.AddRange(list);
                });
            }
        }
        #region AddRootCommand
        public ICommand AddRootCommand { get { return new RelayCommand(() => { if (CanAddRoot()) DataHelper.Default.AddRootAction(ItemsSource); }); } }
        private bool CanAddRoot()
        {
            if (ItemsSource == null || !ItemsSource.Any())
            {
                return true;
            }
            if (ItemsSource != null && ItemsSource.Count == 1 && SingleRoot)
            {
                return false;
            }
            return false;
        }

        #endregion

        #region AddAfterCommand
        public ICommand AddAfterCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    IsAddAfter = true;
                    DataHelper.Default.AddAfterAction(SelectedItems, ItemsSource);
                }, () => SelectedItems.Count == 1);
            }
        }
        #endregion

        #region AddSiblingCommand
        public ICommand AddSiblingCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    IsAddAfter = false;
                    DataHelper.Default.AddSiblingAction(SelectedItems, ItemsSource);
                }, () => SelectedItems.Count == 1);
            }
        }

        #endregion

        #region DeleteCommand
        public ICommand DeleteCommand { get { return new RelayCommand(() => { DataHelper.Default.Delete(SelectedItems, ItemsSource); }, () => SelectedItems.Any()); } }

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
                                var newItem = DataHelper.Default.AddNew("", p.X, p.Y, ItemsSource);
                                ItemsSource.Add(newItem);
                            }
                        }
                    });
            }
        }

        #endregion

        #region Copy Cut Past Command
        public ICommand CopyCommand { get { return new RelayCommand(CopyAction, () => SelectedItems.Any()); } }

        void CopyAction()
        {
            DataHelper.Default.Copy(SelectedItems);
            Type = CopyOrPasteType.Copy;
        }
        public ICommand CutCommand { get { return new RelayCommand(CutAction, () => SelectedItems.Any()); } }

        void CutAction()
        {
            DataHelper.Default.Cut(SelectedItems, ItemsSource);
            Type = CopyOrPasteType.Cut;
        }
        public ICommand PasteCommand { get { return new RelayCommand(PasteAction, () => SelectedItems.Any()); } }
        void PasteAction()
        {
            IsAddAfter = true;
            var dataOnClipBoard = Clipboard.GetDataObject();
            if (dataOnClipBoard == null) return;
            var copyedItemDatas = dataOnClipBoard.GetData(typeof(List<ItemData>)) as List<ItemData>;//从剪切板中取得数据
            if (copyedItemDatas == null) { return; }
            var pasteToItemDatas = SelectedItems.ToList();
            if (!pasteToItemDatas.Any()) { return; }
            DataHelper.Default.Paste(pasteToItemDatas, copyedItemDatas, Type, ItemsSource, ClickPoint);
            Type = CopyOrPasteType.None;
        }

        #endregion
        #endregion
    }
}
