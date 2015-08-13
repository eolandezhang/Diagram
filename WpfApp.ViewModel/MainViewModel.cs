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
using QPP.Wpf.UI.TreeEditor;
using WpfApp.ViewModel.App_Data;
using System.Diagnostics;

namespace WpfApp.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public int Num { get { return Get<int>("Num"); } set { Set("Num", value); } }
        public string Title { get { return Get<string>("Title"); } set { Set("Title", value); } }
        public bool SingleRoot { get { return Get<bool>("SingleRoot"); } set { Set("SingleRoot", value); } }
        public RangeObservableCollection<ItemData> ItemsSource { get; set; }
        public ObservableCollection<ItemData> SelectedItems { get; set; }
        public ObservableCollection<DesignerItem> SelectedDesignerItems { get; set; }
        public ObservableCollection<ItemData> DeletedItems { get; set; }
        public ItemData SelectedItem { get { return Get<ItemData>("SelectedItem"); } set { Set("SelectedItem", value); } }
        public DesignerItem SelectedDesignerItem { get { return Get<DesignerItem>("SelectedDesignerItem"); } set { Set("SelectedDesignerItem", value); } }
        public bool IsAddAfter { get { return Get<bool>("IsAddAfter"); } set { Set("IsAddAfter", value); } }
        public Point ClickPoint { get { return Get<Point>("ClickPoint"); } set { Set("ClickPoint", value); } }
        private string _type = CopyOrPasteType.None;
        public string Type { get { return _type; } set { _type = value; } }
        public ObservableCollection<Color> BorderColors { get { return Get<ObservableCollection<Color>>("BorderColors"); } set { Set("BorderColors", value); } }
        public ObservableCollection<Color> BackgroundColors { get { return Get<ObservableCollection<Color>>("BackgroundColors"); } set { Set("BackgroundColors", value); } }
        public ObservableCollection<ImageUrl> Images { get { return Get<ObservableCollection<ImageUrl>>("Images"); } set { Set("Images", value); } }

        public MainViewModel()
        {
            Title = "Tree Editor";
            SingleRoot = false;
            SelectedItems = new ObservableCollection<ItemData>();
            DeletedItems = new ObservableCollection<ItemData>();
            ItemsSource = new RangeObservableCollection<ItemData>();
            SelectedDesignerItems = new ObservableCollection<DesignerItem>();
            SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
            SelectedDesignerItems.CollectionChanged += SelectedDesignerItems_CollectionChanged;
            BorderColors = new ObservableCollection<Color>()
            {
                new Color("#FF87CEEB"),new Color("#FFFF0000"),new Color("#000000FF")
            };
            BackgroundColors = new ObservableCollection<Color>()
            {
                new Color("#FFA4FFC1"),new Color("#FFF3F781"),new Color("#000000FF")
            };
            Images = new ObservableCollection<ImageUrl>()
            {
               new ImageUrl("Images/fix.png"),
                new ImageUrl("Images/blue.png"),
                new ImageUrl("Images/green.png")
            };
        }



        public class Image
        {
            public Image(string img)
            {
                ItemImage = img;
            }
            public string ItemImage { get; set; }
        }
        public class Color
        {
            public Color(string color)
            {
                ItemColor = color;
            }
            public string ItemColor { get; set; }
        }
        private void SelectedDesignerItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var item = SelectedDesignerItems.FirstOrDefault();
                if (item == null) return;
                var data = item;
                SelectedDesignerItem = data;
            }
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
                        new ItemData("0", "", "0.0", "Root　Item1","{'BorderBrush':'#FF87CEEB','Background':'#000000FF','ImageUrl':[{'Url':'Images/fix.png'},{'Url':'Images/blue.png'}]}"),
                        new ItemData("1", "0", "1.1", "-","{'BorderBrush':'#FF87CEEB','Background':'#000000FF','ImageUrl':[{'Url':'Images/fix.png'}]}"),
                        new ItemData("3", "1", "2.1", "-","{'BorderBrush':'#FF87CEEB','Background':'#000000FF','ImageUrl':[{'Url':'Images/fix.png'}]}"),
                        new ItemData("5", "3", "2.2", "-","{'BorderBrush':'#FF87CEEB','Background':'#000000FF','ImageUrl':[{'Url':'Images/fix.png'}]}"),
                        new ItemData("2", "0", "1.2", "-","{'BorderBrush':'#FF87CEEB','Background':'#000000FF','ImageUrl':[{'Url':'Images/fix.png'}]}"),
                        new ItemData("4", "2", "3.1", "-","{'BorderBrush':'#FF87CEEB','Background':'#000000FF','ImageUrl':[{'Url':'Images/fix.png'}]}"),
                        new ItemData("7", "4", "3.2", "-","{'BorderBrush':'#FF87CEEB','Background':'#000000FF','ImageUrl':[{'Url':'Images/fix.png'}]}"),
                        new ItemData("6", "0", "1.3", "-","{'BorderBrush':'#FFFF0000','Background':'#000000FF','ImageUrl':[{'Url':'Images/fix.png'}]}")
                    };
                    for (var i = 0; i < Num; i++)
                    {
                        list.Add(new ItemData(Guid.NewGuid().ToString(), "0", "Item " + i, "", "{'BorderBrush':'#FF87CEEB','Background':'#000000FF','ImageUrl':[{'Url':'Images/fix.png'}]}"));
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

        #region AddImageCommand
        public ICommand AddImageCommand
        {
            get { return new RelayCommand<ImageUrl>(AddImageAction); }
        }

        private void AddImageAction(ImageUrl imageUrl)
        {
            var imgList = SelectedDesignerItem.ItemStyle.ImageUrl;
            if (imgList.All(x => !x.Url.Equals(imageUrl.Url, StringComparison.OrdinalIgnoreCase)))
            {
                imgList.Add(imageUrl);
            }
        }
        #endregion
        #endregion
    }
}
