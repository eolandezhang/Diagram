using QPP;
using QPP.Command;
using QPP.ComponentModel;
using QPP.Wpf.Command;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
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
        public MainViewModel()
        {
            SingleRoot = false;
            SelectedItems = new ObservableCollection<ItemData>();
            DeletedItems = new ObservableCollection<ItemData>();
            Title = "Tree Editor";
            LoadData();

            //SelectedItems.CollectionChanged += (d, e) =>
            //{
            //    if (e.Action == NotifyCollectionChangedAction.Add)
            //    {
            //        var item = SelectedItems.FirstOrDefault();
            //        if (item != null)
            //        {
            //            var data = item as ItemData;
            //            if (data != null)
            //            {
            //                MessageBox.Show(data.Text);
            //            }
            //        }

            //    }
            //};
        }

        private void LoadData()
        {
            ItemsSource = ItemDataRepository.Default.DataCollection;
        }

        #region Commands
        public ICommand ReloadCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ItemsSource.Clear();
                    var list = new ObservableCollection<ItemData>()
                        {
                    new ItemData("0", "", "0.0", "Root　Item1"),
                    new ItemData("1", "0", "1.1", "-"),
                    new ItemData("3", "1", "2.1", "-"),
                    new ItemData("5", "3", "2.2", "-"),
                    new ItemData("2", "0", "1.2", "-"),
                    new ItemData("4", "2", "3.1", "-"),
                    //new ItemData("6","0", "1.3", "-"),
                    new ItemData("7", "4", "3.2", "-"),
                        };
                    foreach (var itemData in list)
                    {
                        ItemsSource.Add(itemData);
                    }
                });
            }
        }
        #region AddRootCommand

        public ICommand AddRootCommand => new RelayCommand(AddRootAction, CanAddRoot);

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
        public ICommand AddAfterCommand => new RelayCommand(AddAfterAction);

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

        public ICommand AddSiblingCommand => new RelayCommand(AddSiblingAction);

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

        public ICommand DeleteCommand => new RelayCommand(DeleteAction);

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
        #region ShwoMessage

        public ICommand ShowMessageCommand
        {
            get { return new RelayCommand(() => { MessageBox.Show("Show Message."); }); }
        }
        #endregion
        #endregion
    }
}
