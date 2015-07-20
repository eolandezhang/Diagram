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
        public ObservableCollection<ItemData> ItemDatas { get; set; }
        public ObservableCollection<ItemData> SelectedItems { get; set; }
        public MainViewModel()
        {
            SelectedItems=new ObservableCollection<ItemData>();
            Title = "Tree Editor";
            LoadData();
        }

        private void LoadData()
        {
            ItemDatas = new ObservableCollection<ItemData>();
            var list = ItemDataRepository.Default.DataCollection;
            foreach (var itemData in list)
            {
                ItemDatas.Add(itemData);
            }
            //TreeNodeCollection = DataToTreeNodeAdapter.Default.CreateNodes(ItemDataRepository.Default.DataCollection);
        }
    }
}
