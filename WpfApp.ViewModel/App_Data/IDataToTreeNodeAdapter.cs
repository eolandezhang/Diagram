/* ============================================================================== 
* 类名称：IDataToTreeNodeAdapter 
* 类描述：用户需要实现此接口。用于将对象转换为TreeNode
* 创建人：eolandecheung 
* 创建时间：2015/7/16 17:10:54 
* 修改人： 
* 修改时间： 
* 修改备注： 
* @version 1.0 
* ==============================================================================*/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using QPP.Wpf.ComponentModel;
using QPP.Wpf.UI.TreeEditor;

namespace WpfApp.ViewModel.App_Data
{
    public interface IDataToTreeNodeAdapter<T>
    {
        ObservableCollection<TreeItemNode> CreateNodes(List<T> data);
    }
}
