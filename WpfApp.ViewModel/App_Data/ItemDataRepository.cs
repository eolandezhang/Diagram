/* ============================================================================== 
* 类名称：ItemDataRepository 
* 类描述： 
* 创建人：eolandecheung 
* 创建时间：2015/7/16 16:42:52 
* 修改人： 
* 修改时间： 
* 修改备注： 
* @version 1.0 
* ==============================================================================*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WpfApp.ViewModel.App_Data
{
    public class ItemDataRepository
    {
        public static ItemDataRepository Default;
        public ObservableCollection<ItemData> DataCollection { get; set; }


        static ItemDataRepository()
        {
            Default = new ItemDataRepository();
        }

        public ItemDataRepository()
        {
            DataCollection = new ObservableCollection<ItemData>()
            {
                new ItemData("0","","0","Root　Item1"),
                new ItemData("1","0", "1", "1"),
                new ItemData("2","0", "2", "2"),
                new ItemData("3","1", "3", "3"),
                new ItemData("4","2", "4", "4"),
                new ItemData("5","3", "5\r\nasdf", "5")
            };
        }

        public ItemData AddNew(string Pid)
        {
            return new ItemData(Guid.NewGuid().ToString(), Pid, "Item", "");
        }
    }
}
