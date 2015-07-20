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
using System.Linq;
using System.Text;

namespace WpfApp.ViewModel.App_Data
{
    public class ItemDataRepository
    {
        public static ItemDataRepository Default;
        public List<ItemData> DataCollection { get; set; }


        static ItemDataRepository()
        {
            Default = new ItemDataRepository();
        }

        public ItemDataRepository()
        {
            DataCollection = new List<ItemData>()
            {
                new ItemData("d342e6d4-9e76-4a21-b4f8-41f8fab0f93c","","Root1","Root　Item1"),
                new ItemData("d342e6d4-9e76-4a21-b4f8-41f8fab0f931","d342e6d4-9e76-4a21-b4f8-41f8fab0f93c", "Item-1_asdfasdfasdfasdfasdfasdfasdfasdfasdfasdf", "1"),
                new ItemData("d342e6d4-9e76-4a21-b4f8-41f8fab0f932","d342e6d4-9e76-4a21-b4f8-41f8fab0f93c", "Item-2", "2"),
                new ItemData("d342e6d4-9e76-4a21-b4f8-41f8fab0f933","d342e6d4-9e76-4a21-b4f8-41f8fab0f931", "Item-3", "3"),
                new ItemData("d342e6d4-9e76-4a21-b4f8-41f8fab0f934","d342e6d4-9e76-4a21-b4f8-41f8fab0f93c", "Item-4", "4"),
                new ItemData("d342e6d4-9e76-4a21-b4f8-41f8fab0f935","d342e6d4-9e76-4a21-b4f8-41f8fab0f933", "Item-5\r\nasdf", "5")//,
                //new ItemData("d342e6d4-9e76-4a21-b4f8-41f8fab0f93a","","Root2","Root　Item2")
            };
        }
    }
}
