using QPP.Wpf.UI.TreeEditor;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WpfApp.ViewModel.App_Data
{
    public interface IDataToTreeNodeAdapter<T>
    {
        ObservableCollection<TreeItemNode> CreateNodes(List<T> data);
    }
}
