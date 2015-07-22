using QPP;
using QPP.Command;
using QPP.ComponentModel;
using QPP.Wpf.Command;
using System.Collections.ObjectModel;
using System.Linq;
using WpfApp.ViewModel.App_Data;

namespace WpfApp.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public string Title { get { return Get<string>("Title"); } set { Set("Title", value); } }
        public ObservableCollection<ItemData> ItemsSource { get; set; }
        public ObservableCollection<ItemData> SelectedItems { get; set; }
        public ObservableCollection<ItemData> DeletedItems { get; set; }
        public MainViewModel()
        {
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
        #region AddAfterCommand
        public ICommand AddAfterCommand
        {
            get { return new RelayCommand(AddAfterAction); }
        }
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
        #region AddSiblingCommand

        public ICommand AddSiblingCommand
        {
            get { return new RelayCommand(AddSiblingAction); }
        }

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

        public ICommand DeleteCommand
        {
            get { return new RelayCommand(DeleteAction); }
        }

        private void DeleteAction()
        {
            var selectedItem = GetSelectedItem();
            if (selectedItem != null)
            {
                if (selectedItem.ItemParentId.IsNullOrEmpty()) return;
                ItemsSource.Remove(selectedItem);
            }
        }
        #endregion

        #endregion
    }
}
