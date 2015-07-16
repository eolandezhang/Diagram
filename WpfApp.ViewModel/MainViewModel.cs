﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using QPP.ComponentModel;
using QPP.Wpf.ComponentModel;
using QPP.Wpf.UI.TreeEditor;
using WpfApp.ViewModel.App_Data;

namespace WpfApp.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public string Title { get { return Get<string>("Title"); } set { Set("Title", value); } }
        public ObservableCollection<TreeItemNode> TreeNodeCollection { get; set; }
        public MainViewModel()
        {
            Title = "Tree Editor";
            LoadData();
        }

        private void LoadData()
        {
            TreeNodeCollection = DataToTreeNodeAdapter.Default.CreateNodes(ItemDataRepository.Default.DataCollection);
        }
    }
}