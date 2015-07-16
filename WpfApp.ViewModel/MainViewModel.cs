using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QPP.ComponentModel;

namespace WpfApp.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public string Title { get { return Get<string>("Title"); } set { Set("Title", value); } }

        public MainViewModel()
        {
            Title = "Tree Editor";
        }
    }
}
