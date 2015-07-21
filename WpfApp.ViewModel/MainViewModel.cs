using System.Collections.Generic;
using QPP.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using QPP.Command;
using QPP.Wpf.Command;
using WpfApp.ViewModel.App_Data;

namespace WpfApp.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public string Title { get { return Get<string>("Title"); } set { Set("Title", value); } }
        //public ObservableCollection<TreeItemNode> TreeNodeCollection { get; set; }
        public ObservableCollection<ItemData> ItemsSource
        {
            get;
            private set;
            //get { return Get<ObservableCollection<ItemData>>("ItemsSource"); }
            //set { Set("ItemsSource", value); }
        }
        public ObservableCollection<object> SelectedItems { get; set; }
        public MainViewModel()
        {
            SelectedItems = new ObservableCollection<object>();
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
            
            ItemsSource = new ObservableCollection<ItemData>()
            {
                new ItemData("0","","0","Root　Item1"),
                new ItemData("1","0", "1", "1"),
                new ItemData("2","0", "2", "2"),
                new ItemData("3","1", "3", "3"),
                new ItemData("4","2", "4", "4"),
                new ItemData("5","3", "5\r\nasdf", "5")
            };
        }

        #region Commands



        ItemData GetSelectedItem()
        {

            if (SelectedItems.Count == 1)
            {
                var item = SelectedItems.FirstOrDefault();
                var selectedItem = item as ItemData;
                return selectedItem;
            }
            return null;

        }
        #region AddNew

        public ICommand AddNew
        {
            get { return new RelayCommand(AddNewAction); }
        }

        void AddNewAction()
        {
            var selectedItem = GetSelectedItem();
            if (selectedItem != null)
            {
                var newItem = ItemDataRepository.Default.AddNew(selectedItem.ItemId);
                ItemsSource.Add(newItem);
            }
        }
        #endregion
        //#region Load
        //public ICommand Load
        //{
        //    get { return new RelayCommand(LoadAction); }
        //}

        //void LoadAction()
        //{
        //    LoadData();
        //}
        //#endregion
        #endregion
    }
}
