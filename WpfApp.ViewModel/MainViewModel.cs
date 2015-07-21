using System.Collections.Generic;
using QPP.ComponentModel;
using System.Collections.ObjectModel;
using WpfApp.ViewModel.App_Data;

namespace WpfApp.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public string Title { get { return Get<string>("Title"); } set { Set("Title", value); } }
        //public ObservableCollection<TreeItemNode> TreeNodeCollection { get; set; }
        public ObservableCollection<ItemData> ItemsSource { get; set; }
        public ObservableCollection<object> SelectedItems { get; set; }
        public MainViewModel()
        {
            SelectedItems = new ObservableCollection<object>();
            Title = "Tree Editor";
            LoadData();
        }

        private void LoadData()
        {
            ItemsSource = new ObservableCollection<ItemData>();
            var list = ItemDataRepository.Default.DataCollection;
            foreach (var itemData in list)
            {
                ItemsSource.Add(itemData);
            }
            //TreeNodeCollection = DataToTreeNodeAdapter.Default.CreateNodes(ItemDataRepository.Default.DataCollection);
        }
    }
}
